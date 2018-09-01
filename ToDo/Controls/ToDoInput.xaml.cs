using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ToDo.Controls
{
    /// <summary>
    /// Interaction logic for ToDoInput.xaml
    /// </summary>
    public partial class ToDoInput : UserControl
    {
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ToDoInput), new PropertyMetadata("Input"));
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register(nameof(Checked), typeof(bool), typeof(ToDoInput), new PropertyMetadata(false, (o, e) => {
            if (o is ToDoInput _this && e.NewValue is bool newVal)
            {
                if (newVal == true)
                {
                    _this.Input.IsEnabled = false;
                    _this.Opacity = 0.8;
                    BrushConverter bc = new BrushConverter();
                    _this.CheckButton.Foreground = (Brush)bc.ConvertFrom("#3D3");
                }
                else
                {
                    _this.Input.IsEnabled = true;
                    _this.Opacity = 1;
                    _this.CheckButton.Foreground = _this.DefaultCheckColor;
                }
            }
        }));

        public EventHandler<KeyEventArgs> InputKeyDown { get; set; }
        //public EventHandler<RoutedEventArgs> CheckClicked { get; set; }
        public EventHandler<RoutedEventArgs> RemoveClicked { get; set; }
        public int TabSize { get; set; } = 0;
        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        private Brush DefaultCheckColor { get; set; }

        private const int MAX_TAB_SIZE = 10;

        public ToDoInput() { Init(); }

        public ToDoInput(int tabSize)
        {
            Init();
            TabSize = tabSize;
            UpdateTabMargins();
        }

        public ToDoInput(string placeholder)
        {
            Init();
            PlaceholderText = placeholder;
        }

        private void Init()
        {
            InitializeComponent();

            Input.KeyDown += Input_KeyDown;

            Margin = new Thickness(0, 10, 0, 0);

            DataContext = this;

            CheckButton.Click += CheckButton_Click;
            RemoveButton.Click += RemoveButton_Click;

            Container.MouseEnter += Container_MouseEnter;
            Container.MouseLeave += Container_MouseLeave;

            DefaultCheckColor = CheckButton.Foreground;
        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            RemoveButton.Visibility = Visibility.Hidden;
        }

        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveButton.Visibility = Visibility.Visible;
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            Checked = !Checked;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this, e);
        }

        public void Tab()
        {
            TabSize++;
            if (TabSize > MAX_TAB_SIZE)
            {
                TabSize = MAX_TAB_SIZE;
            }
            UpdateTabMargins();
        }

        public void BackTab()
        {
            TabSize--;
            if (TabSize < 0)
            {
                TabSize = 0;
            }
            UpdateTabMargins();
        }

        private void UpdateTabMargins()
        {
            var margins = Container.Margin;
            Input.MinWidth = 400 - (30 * TabSize);
            Container.Margin = new Thickness(30 * TabSize, margins.Top, margins.Right, margins.Bottom);
        }

        /// <summary>
        /// focuses on input
        /// </summary>
        public new void Focus()
        {
            Input.Focus();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            InputKeyDown?.Invoke(this, e);
        }
    }
}
