﻿using System.Windows;
using System.Windows.Controls;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for CarAndProgressUserControl.xaml
    /// </summary>
    public partial class CarAndProgressUserControl : UserControl
    {
        public CarAndProgressUserControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty CAPUCProgressProperty = DependencyProperty.Register("CAPUCProgress", typeof(int), typeof(CarAndProgressUserControl));

        public int CAPUCProgress
        {
            get { return (int)GetValue(CAPUCProgressProperty); }
            set
            {
                SetValue(CAPUCProgressProperty, value);
            }
        }

        public static DependencyProperty CAPUCSliderProgressProperty = DependencyProperty.Register("CAPUCSliderProgress", typeof(int), typeof(CarAndProgressUserControl));

        public int CAPUCSliderProgress
        {
            get { return (int)GetValue(CAPUCSliderProgressProperty); }
            set
            {
                SetValue(CAPUCSliderProgressProperty, value);
            }
        }

        public static DependencyProperty CAPUCNameProperty = DependencyProperty.Register("CAPUCName", typeof(string), typeof(CarAndProgressUserControl));

        public string CAPUCName
        {
            get { return (string)GetValue(CAPUCProgressProperty); }
            set
            {
                SetValue(CAPUCProgressProperty, value);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public static DependencyProperty CAPUCSliderStyleProperty = DependencyProperty.Register("CAPUCSliderStyle", typeof(Style), typeof(CarAndProgressUserControl));

        public Style CAPUCSliderStyle
        {
            get { return (Style)GetValue(CAPUCSliderStyleProperty); }
            set
            {
                SetValue(CAPUCSliderStyleProperty, value);
            }
        }

        public static DependencyProperty CAPUCCanBeShownProperty = DependencyProperty.Register("CAPUCCanBeShown", typeof(Visibility), typeof(CarAndProgressUserControl));

        public Visibility CAPUCCanBeShown
        {
            get { return (Visibility)GetValue(CAPUCCanBeShownProperty); }
            set
            {
                SetValue(CAPUCCanBeShownProperty, value);
            }
        }

        public bool CAPUCShowRanking
        {
            get { return (bool)GetValue(CAPUCShowRankingProperty); }
            set { SetValue(CAPUCShowRankingProperty, value); }
        }

        public static readonly DependencyProperty CAPUCShowRankingProperty = DependencyProperty.Register("CAPUCShowRanking", typeof(bool), typeof(CarAndProgressUserControl));

        public string CAPUCRanking
        {
            get { return (string)GetValue(CAPUCRankingProperty); }
            set { SetValue(CAPUCRankingProperty, value); }
        }

        public static readonly DependencyProperty CAPUCRankingProperty = DependencyProperty.Register("CAPUCRanking", typeof(string), typeof(CarAndProgressUserControl));

        public bool CAPUCShowFinishResults
        {
            get { return (bool)GetValue(CAPUCShowFinishResultsProperty); }
            set { SetValue(CAPUCShowFinishResultsProperty, value); }
        }

        public static readonly DependencyProperty CAPUCShowFinishResultsProperty =
            DependencyProperty.Register("CAPUCShowFinishResults", typeof(bool), typeof(CarAndProgressUserControl));

        public string CAPUCAccuracy
        {
            get { return (string)GetValue(CAPUCAccuracyProperty); }
            set { SetValue(CAPUCAccuracyProperty, value); }
        }

        public static readonly DependencyProperty CAPUCAccuracyProperty =
            DependencyProperty.Register("CAPUCAccuracy", typeof(string), typeof(CarAndProgressUserControl));
    }
}