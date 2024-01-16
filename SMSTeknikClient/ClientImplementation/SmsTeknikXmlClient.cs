using System.Text;
using SMSTeknikClient.Config;
using SMSTeknikClient.Messages;
using static System.Globalization.CultureInfo;
using static SMSTeknikClient.ClientImplementation.Validation;
using XE = System.Xml.Linq.XElement;

// ReSharper disable StringLiteralTypo

namespace SMSTeknikClient.ClientImplementation;

/// <inheritdoc />
public class SmsTeknikXmlClient : ISmsTeknikClient
{
	private readonly HttpClient _client;

	public SmsTeknikXmlClient(IHttpClientFactory clientFactory)
		=> _client = clientFactory.CreateClient();

	/// <summary>
	/// Use <see cref="SmsTeknik" /> to create a client
	/// </summary>
	/// <param name="config">passed down config from <see cref="SmsTeknik" /></param>
	internal SmsTeknikXmlClient(SmsTeknikConfiguration config)
	{
		_client = new();
		_client.DefaultRequestHeaders.Add("Authorization",
				$"Basic {Utils.Base64Encode($"{config.Username}:{config.Password}")}");
		_client.BaseAddress = new("https://api.smsteknik.se/");
	}

	/// <inheritdoc />
	public async Task<MessageResponse> Send(OutgoingSmsMessage message, CancellationToken cancellationToken = default) =>
			(await Send(new SendRequest(message), cancellationToken).ConfigureAwait(false))
			.MessageResponses.Single();

	/// <inheritdoc />
	public async Task<SendResponse> Send(SendRequest sendRequest, CancellationToken cancellationToken = default)
	{
		var validationErrors = GetValidationErrors(sendRequest);
		if (validationErrors.Any())
			throw new ArgumentException("Invalid SMS send request: " + string.Join(", ", validationErrors));

		var xml = CreateXmlPayload(sendRequest);

		using var content = new StringContent(xml.ToString(), Encoding.UTF8, "application/xml");
		using var responseMessage =
				await _client.PostAsync("send/xml/", content, cancellationToken).ConfigureAwait(false);
		responseMessage.EnsureSuccessStatusCode();
		var result = await responseMessage.Content.ReadAsStringAsync();
		return ParseResult(result, sendRequest.OutgoingSmsMessages);
	}

	/// <inheritdoc />
	public async Task<int> CheckCredits(CancellationToken cancellationToken = default)
	{
		using var responseMessage = await _client.GetAsync("utilities/checkcredits/", cancellationToken);
		responseMessage.EnsureSuccessStatusCode();
		var result = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

		return int.TryParse(result, out var parsedResult)
				? parsedResult
				: throw new("No access");
	}

	public void Dispose()
	{
		_client.Dispose();
		GC.SuppressFinalize(this);
	}

	private static XE CreateXmlPayload(SendRequest sendRequest)
	{
		var xmlItems = new XE("items");
		var req = sendRequest.OutgoingSmsMessages.First();
		var xml = new XE("sms-teknik",
				new XE("operationtype", 0),
				new XE("smssender", req.From),
				new XE("multisms", 1),
				new XE("maxmultisms", 0),
				new XE("send_date", req.SendAt?.ToString("yyyy-MM-dd", InvariantCulture) ?? ""),
				new XE("send_time", req.SendAt?.ToString("HH:dd:ss", InvariantCulture) ?? ""),
				new XE("udmessage", req.Body),
				xmlItems
		);

		if (!string.IsNullOrEmpty(req.StatusCallBackUrl))
		{
			xml.Add(new XE("deliverystatustype", "3"),
					new XE("deliverystatusaddress", req.StatusCallBackUrl));
		}

		xmlItems.Add(sendRequest.OutgoingSmsMessages.Select(outgoingMessage
				=> new XE("recipient", new XE("nr", outgoingMessage.To))));
		return xml;
	}

	private static SendResponse ParseResult(string result, ReadOnlySpan<OutgoingSmsMessage> outgoingSmsMessages)
	{
		var arrResult = result.Split(';');

		if (outgoingSmsMessages.Length > 1 && arrResult.Length == 1)
		{
			// We expected to receive multiple response codes (one for each message), but we only receive one.
			// This means that the entire request failed (eg. wrong credentials, parse error or quota exceeded).
			// We have two options: 1) throw exception, or 2) populate the same error message for each of the message response.
			// This scenario only applies when sending to multiple receivers.
			// Example error codes: 0:Access denied, 0:No Valid recipients
			var errorCode = arrResult.Single().Remove(0, 2);
			throw new("Error: " + errorCode);
		}

		if (outgoingSmsMessages.Length != arrResult.Length)
			throw new($"Message length mismatch: {outgoingSmsMessages.Length} != {arrResult.Length}...");

		var messageResponses = new MessageResponse[outgoingSmsMessages.Length];

		for (var i = 0; i < messageResponses.Length; i++)
		{
			var resultItem = arrResult[i];

			bool success;
			string? errorMessage = null;
			long? smsId = null;

			if (resultItem.StartsWith("0:"))
			{
				// Error
				success = false;
				errorMessage = resultItem.Remove(0, 2);
			}
			else
			{
				// Success
				success = true;
				smsId = Convert.ToInt64(resultItem);
			}

			messageResponses[i] = new(outgoingSmsMessages[i], success, errorMessage, smsId);
		}

		return new(messageResponses);
	}
}