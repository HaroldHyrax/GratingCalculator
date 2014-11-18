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

namespace Grating_Calculator
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                UpdateValues();
            }
        }

        public void UpdateValues()
        {
            try
            {
                // Update values
                double barHeight = double.Parse(barHeightBox.Text) / 1000;
                double barThickness = double.Parse(barThicknessBox.Text) / 1000;
                double barPitch = double.Parse(barPitchBox.Text) / 1000;
                double span = double.Parse(spanBox.Text) / 1000;
                double allowableStress = double.Parse(allowableStressBox.Text) * 1000000;
                double youngsModulus = double.Parse(youngsModulusBox.Text) * 1000000000;
                double limDeflection = double.Parse(limDeflectionBox.Text);

                // Calculate outputs
                double inertia = Calculations.CalcInertia(barThickness, barHeight);
                double bendingMoment = Calculations.CalcMaxBendYield(allowableStress, inertia, barHeight);
                string lineBendingForce = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcLineBendingForce(bendingMoment, span), barPitch));
                string lineDefl = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcLineDefl(span, inertia, youngsModulus, limDeflection), barPitch));
                string lineMaxdefl = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcLineMaxDefl(0.01, span, inertia, youngsModulus), barPitch));
                string distBendingForce = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcDistBendingForce(bendingMoment, span), barPitch));
                string distDefl = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcDistDefl(span, inertia, youngsModulus, limDeflection), barPitch));
                string distMaxDefl = System.Convert.ToString(Calculations.conditionForDisplay(Calculations.CalcDistMaxDefl(0.01, span, youngsModulus, inertia), barPitch));

                // Determine limiting factor and highlight in red
                if (float.Parse(lineBendingForce) < float.Parse(lineDefl) && float.Parse(lineBendingForce) < float.Parse(lineMaxdefl))
                {
                    lineBendingForceBox.Foreground = Brushes.Red;
                    lineDeflBox.Foreground = Brushes.Gray;
                    lineMaxDeflBox.Foreground = Brushes.Gray;
                }
                else if (float.Parse(lineDefl) < float.Parse(lineBendingForce) && float.Parse(lineDefl) < float.Parse(lineMaxdefl))
                {
                    lineBendingForceBox.Foreground = Brushes.Gray;
                    lineDeflBox.Foreground = Brushes.Red;
                    lineMaxDeflBox.Foreground = Brushes.Gray;
                }
                else
                {
                    lineBendingForceBox.Foreground = Brushes.Gray;
                    lineDeflBox.Foreground = Brushes.Gray;
                    lineMaxDeflBox.Foreground = Brushes.Red;
                }

                if (float.Parse(distBendingForce) < float.Parse(distDefl) && float.Parse(distBendingForce) < float.Parse(distMaxDefl))
                {
                    distBendingForceBox.Foreground = Brushes.Red;
                    distDeflBox.Foreground = Brushes.Gray;
                    distMaxDeflBox.Foreground = Brushes.Gray;
                }
                else if (float.Parse(distDefl) < float.Parse(distBendingForce) && float.Parse(distDefl) < float.Parse(distMaxDefl))
                {
                    distBendingForceBox.Foreground = Brushes.Gray;
                    distDeflBox.Foreground = Brushes.Red;
                    distMaxDeflBox.Foreground = Brushes.Gray;
                }
                else
                {
                    distBendingForceBox.Foreground = Brushes.Gray;
                    distDeflBox.Foreground = Brushes.Gray;
                    distMaxDeflBox.Foreground = Brushes.Red;
                }

                // Update output textboxes
                lineBendingForceBox.Text = lineBendingForce;
                lineDeflBox.Text = lineDefl;
                lineMaxDeflBox.Text = lineMaxdefl;
                distBendingForceBox.Text = distBendingForce;
                distDeflBox.Text = distDefl;
                distMaxDeflBox.Text = distMaxDefl;
            }
            catch
            {
                MessageBox.Show("Invalid value detected.\nPlease ensure all values contain only numbers\nand correlate to the respective units shown.");
            }
        }
        public static class Calculations
        {
            // General calculations:
            public static float conditionForDisplay(double inputValue, double pitch)
            {
                return (float)System.Math.Round((inputValue * ((1 / pitch) / 9.81)), 2);
            }
            public static double CalcInertia(double width, double height)
            {
                return (width * System.Math.Pow(height, 3)) / 12;
            }
            public static double CalcMaxBendYield(double allowableStress, double inertia, double height)
            {
                return (allowableStress * inertia) / (height / 2);
            }
            // Line load calculations:
            public static double CalcLineBendingForce(double bendingMoment, double span)
            {
                return ((4 * bendingMoment) / span);
            }
            public static double CalcLineDefl(double span, double inertia, double youngs, double ratio)
            {
                double defRatio = span / ratio;
                return (defRatio * 48 * youngs * inertia) / System.Math.Pow(span, 3);
            }
            public static double CalcLineMaxDefl(double limMaxDeflection, double span, double inertia, double youngs)
            {
                return ((48 * limMaxDeflection * youngs * inertia) / System.Math.Pow(span, 3));
            }
            // Distributed load calculations:
            public static double CalcDistBendingForce(double bendingMoment, double span)
            {
                return ((8 * bendingMoment) / span);
            }
            public static double CalcDistDefl(double span, double inertia, double youngs, double ratio)
            {
                double defRatio = span / ratio;
                return ((defRatio * 384 * youngs * inertia) / (5 * System.Math.Pow(span, 3)));
            }
            public static double CalcDistMaxDefl(double limMaxDeflection, double span, double youngs, double inertia)
            {
                return ((384 * limMaxDeflection * youngs * inertia) / (5 * System.Math.Pow(span, 3)));
            }
        }


        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
        }
    }

}
