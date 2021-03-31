using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	[JsonConverter(typeof(FLiRSConverter))]
	public enum FLiRS
	{
		Unspecified,
		ValueFalse,

		[ObsoleteAttribute("As of schema version 3, the remote zwave js websocket server replaced true with Value250ms and Value1000ms. This enum value is provided for backwards compatability with schema 2 and below. This enum value will be removed in a future version of this product.")]
		ValueLegacyTrue,
		Value250ms,
		Value1000ms
	}

	public class FLiRSConverter : JsonConverter<FLiRS>
	{
		public override FLiRS Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.String:
					string jsonValue = reader.GetString();

					if (jsonValue.ToUpperInvariant() == "250MS")
					{
						return FLiRS.Value250ms;
					}
					else if (jsonValue.ToUpperInvariant() == "1000MS")
					{
						return FLiRS.Value1000ms;
					}

					throw new NotSupportedException(String.Format("Property has invalid value for type FLiRS. The property value passed in was {0}.", jsonValue));

				case JsonTokenType.True:

					return FLiRS.ValueLegacyTrue;
				case JsonTokenType.False:
					return FLiRS.ValueFalse;
				default:
					throw new JsonException();
			}
		}

		public override void Write(Utf8JsonWriter writer, FLiRS value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case FLiRS.ValueLegacyTrue:
					writer.WriteStringValue("true");
					break;
				case FLiRS.ValueFalse:
					writer.WriteStringValue("false");
					break;
				case FLiRS.Value1000ms:
					writer.WriteStringValue("1000ms");
					break;
				case FLiRS.Value250ms:
					writer.WriteStringValue("250ms");
					break;
				case FLiRS.Unspecified:
					writer.WriteNullValue();
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}