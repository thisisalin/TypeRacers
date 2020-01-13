﻿using System;
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

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for IOUserControl.xaml
    /// </summary>
    public partial class ReadAndTypeUserControl : UserControl
    {
        public ReadAndTypeUserControl()
        {
            InitializeComponent();
        }

        //dp for typed text properties
        public readonly static DependencyProperty RATUCInlinesProperty = DependencyProperty.Register("RATUCInlines", typeof(IEnumerable<Inline>), typeof(ReadAndTypeUserControl));

        public IEnumerable<Inline> RATUCInlines
        {
            get { return (IEnumerable<Inline>)GetValue(RATUCInlinesProperty); }
            set
            {
                SetValue(RATUCInlinesProperty, value);
            }
        }

        //dp for textbox text
        public readonly static DependencyProperty RATUCCurrentInputTextProperty = DependencyProperty.Register("RATUCCurrentInputText", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCCurrentInputText
        {
            get { return (string)GetValue(RATUCCurrentInputTextProperty); }
            set
            {
                SetValue(RATUCCurrentInputTextProperty, value);
            }
        }

        //dp for all text typed
        public readonly static DependencyProperty RATUCAllTextTypedProperty = DependencyProperty.Register("RATUCAllTextTyped", typeof(bool), typeof(ReadAndTypeUserControl));

        public bool RATUCAllTextTyped
        {
            get { return (bool)GetValue(RATUCAllTextTypedProperty); }
            set
            {
                SetValue(RATUCAllTextTypedProperty, value);
            }
        }

        //dp for background color in text box
        public readonly static DependencyProperty RATUCBackgroundColorProperty = DependencyProperty.Register("RATUCBackgroundColor", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCBackgroundColor
        {
            get { return (string)GetValue(RATUCBackgroundColorProperty); }
            set
            {
                SetValue(RATUCBackgroundColorProperty, value);
            }
        }


        public readonly static DependencyProperty RATUCTypingAlertProperty = DependencyProperty.Register("RATUCTypingAlert", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCTypingAlert
        {
            get { return (string)GetValue(RATUCTypingAlertProperty); }
            set
            {
                SetValue(RATUCTypingAlertProperty, value);
            }
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

    }
}
