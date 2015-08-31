using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Windows.UI.Xaml.Data;

namespace BottomSlideContentPresenterSample
{
    /// <summary>
    /// Basic converter to show localizations on list. Needed only for this sample.
    /// </summary>
   public  class MapAddressToStringConverter: IValueConverter
    {
       public object Convert(object value, Type targetType, object parameter, string language)
       {
           if (value is MapLocation)
           {
               var castedLocation = value as MapLocation;
               return string.Format("{0} {1}",castedLocation.Address.Town , castedLocation.Address.Region);
           }
           return string.Empty;
       }

       public object ConvertBack(object value, Type targetType, object parameter, string language)
       {
           throw new NotImplementedException();
       }
    }
}
