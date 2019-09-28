/**
 * Credits to qqbenq (https://stackoverflow.com/users/1552016/qqbenq) for this implementation
 * Link: https://stackoverflow.com/a/19210037
**/

using System;
using System.Windows.Data;

namespace ConverterNamespace
{
	public class BooleanAndConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			foreach (object value in values)
			{
				if ((value is bool) && (bool)value == false)
				{
					return false;
				}

				if (value is int)
				{
					int index = (int)value;
					return (index > 0);
				}
			}
			return true;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException("BooleanAndConverter is a OneWay converter.");
		}
	}

	public class BooleanAndNotConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			foreach (object value in values)
			{
				if ((value is bool) && (bool)value == true)
				{
					return false;
				}

				if (value is int)
				{
					int index = (int)value;
					return (index > 0);
				}
			}
			return true;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException("BooleanAndConverter is a OneWay converter.");
		}
	}
}