using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomPasswordGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Generate_btn_click(object sender, RoutedEventArgs e)
        {
            int MaxPasswordLength;
            string SpecialCharactersToUse;
            string selectedValue;
            bool parseOK;
            //get, parse and store the minimum and maximum password lengths
            ComboBoxItem MaximumLength = (ComboBoxItem)Length_Max.SelectedItem;
            selectedValue = MaximumLength.Content.ToString();
            parseOK = Int32.TryParse(selectedValue, out MaxPasswordLength);

            if(SpecialChar_Chkbox.IsChecked == true)
            {
                SpecialCharactersToUse = "*$-+?_&=!%{}/";
                for (int i = 0; i < 100; i++)
                {
                    Results_txtbox.Text = (RandomPassword.Generate(MaxPasswordLength, SpecialCharactersToUse));
                }
            }
            else
            {
                SpecialCharactersToUse = "ABCDEFGHJKLMNPQRSTWXYZabcdefgijkmnopqrstwxyz23456789";
                for (int i = 0; i < 100; i++)
                {
                    Results_txtbox.Text = (RandomPassword.Generate(MaxPasswordLength, SpecialCharactersToUse));
                }
            }




            
        }//end Generate_btn_click
    }
}
