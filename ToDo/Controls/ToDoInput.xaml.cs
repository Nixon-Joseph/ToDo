using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            set { SetValue(PlaceholderTextProperty, value);  }
        }

        public EventHandler<KeyEventArgs> InputKeyDown { get; set; }
        public int TabSize { get; set; } = 0;

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
