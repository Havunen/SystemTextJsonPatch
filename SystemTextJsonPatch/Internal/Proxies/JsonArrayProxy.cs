using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal.Proxies
{
	internal sealed class JsonArrayProxy : IPropertyProxy
	{
		private readonly JsonArray _jsonArray;
		private readonly string _propertyName;

		internal JsonArrayProxy(JsonArray jsonArray, string propertyName)
		{
			_jsonArray = jsonArray;
			_propertyName = propertyName;
		}

		public object? GetValue(object target)
		{
			var idx = _propertyName == Consts.LastElement ? _jsonArray.Count - 1 : this.PropIndex;
			return _jsonArray[idx];
		}

		public void SetValue(object target, object? convertedValue)
		{
			var value = convertedValue == null ? null : JsonSerializer.SerializeToNode(convertedValue);

			if (_propertyName == Consts.LastElement)
			{
				_jsonArray.Add(value);
			}
			else
			{
				var idx = this.PropIndex;

				_jsonArray.RemoveAt(idx);
				_jsonArray.Insert(idx, value);
			}
		}

		public void RemoveValue(object target)
		{
			if (_propertyName == Consts.LastElement)
			{
				_jsonArray.RemoveAt(_jsonArray.Count - 1);
			}
			else
			{
				_jsonArray.RemoveAt(index: this.PropIndex);
			}
		}

		private int PropIndex => int.Parse(_propertyName, NumberStyles.Number, CultureInfo.InvariantCulture);

		public bool CanRead => true;
		public bool CanWrite => true;
		public Type PropertyType => typeof(JsonNode);
	}
}