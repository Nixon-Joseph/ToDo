using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ToDo.Controls;
using ToDo.Models;

namespace ToDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Debouncer _Debouncer { get; } = new Debouncer();
        private List<ToDoInput> Inputs { get; set; }
        private EventHandler InputsChanged { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            BuildInputs();

            CloseButton.Click += CloseButton_Click;
            MinifyButton.Click += MinifyButton_Click;

            Dragger.MouseDown += Dragger_MouseDown;

            MouseEnter += MainPanel_MouseEnter;
            MouseLeave += MainPanel_MouseLeave;

            InputsChanged += (sender, e) =>
            {
                _Debouncer.Debouce(() => { PersistData(Inputs.Select(i => i.ToString())); });
            };
        }

        private void BuildInputs()
        {
            var stored = GetPersistedData();
            Inputs = new List<ToDoInput>();
            if (stored != null && stored.Count() > 0)
            {
                foreach (var store in stored)
                {
                    SetupNewInput(new ToDoInput(store));
                }
            }
            else
            {
                SetupNewInput(new ToDoInput());
            }
        }

        private void SetupNewInput(ToDoInput input, int index = -1)
        {
            input.PreviewKeyDown += Input_KeyDown;
            input.RemoveClicked += Input_RemoveClicked;
            input.CheckClicked += Input_CheckClicked;
            if (index >= 0)
            {
                Inputs.Insert(index, input);
                InputStack.Children.Insert(index, input);
            }
            else
            {
                Inputs.Add(input);
                InputStack.Children.Add(input);
            }
        }

        private void Input_CheckClicked(object sender, RoutedEventArgs e)
        {
            InputsChanged?.Invoke(this, null);
        }

        private void MainPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            ControlGrid.Opacity = 0;
        }

        private void MainPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlGrid.Opacity = 1;
        }

        private void Dragger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MinifyButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ToDoInput input)
            {
                var index = Inputs.IndexOf(input);
                switch (e.Key)
                {
                    case Key.Enter:
                        AddNewInput(input, index);
                        break;
                    case Key.Up:
                        if (index > 0)
                        {
                            HandleUpDown(index, index - 1);
                            e.Handled = true;
                        }
                        break;
                    case Key.Down:
                        if (index < Inputs.Count - 1)
                        {
                            HandleUpDown(index, index + 1);
                            e.Handled = true;
                        }
                        break;
                    case Key.Tab:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            input.BackTab();
                        }
                        else
                        {
                            input.Tab();
                        }
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
            InputsChanged?.Invoke(this, null);
        }

        private void Input_RemoveClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToDoInput input)
            {
                if (Inputs.Count > 1)
                {
                    var index = Inputs.IndexOf(input);
                    Inputs.RemoveAt(index);
                    InputStack.Children.RemoveAt(index);
                }
                else
                {
                    input.Checked = false;
                    input.ClearText();
                }
                InputsChanged?.Invoke(this, null);
            }
        }

        private bool IsCtrlDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        private void HandleUpDown(int oldIndex, int newIndex)
        {
            if (IsCtrlDown() == true)
            {
                SwapNodes(oldIndex, newIndex);
            }
            else
            {
                Inputs[newIndex].Focus();
            }
        }

        private void SwapNodes(int indexA, int indexB)
        {
            Inputs.Swap(indexA, indexB);
            InputStack.SwapChildren(indexA, indexB);
            InputsChanged?.Invoke(this, null);
        }

        private void AddNewInput(ToDoInput sender, int index)
        {
            var newInput = new ToDoInput(sender.TabSize);
            SetupNewInput(newInput, index + 1);
            newInput.Loaded += (_sender, _e) => { newInput.Focus(); };
            InputsChanged?.Invoke(this, null);
        }

        private int IndexOfInputs(ToDoInput node)
        {
            var index = 0;
            foreach (var linkedNode in Inputs)
            {
                if (linkedNode == node)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        private IEnumerable<InputStorage> GetPersistedData()
        {
            if (File.Exists("./ToDoData/_.txt") == true)
            {
                var stored = new List<InputStorage>();
                var lines = File.ReadAllLines("./ToDoData/_.txt");
                foreach (var line in lines)
                {
                    try
                    {
                        stored.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<InputStorage>(line));
                    }
                    catch (Exception ex) { }
                }
                return stored;
            }
            return null;
        }

        private void PersistData(IEnumerable<string> inputStrings)
        {
            if (Directory.Exists("./ToDoData") == false)
            {
                Directory.CreateDirectory("./ToDoData");
            }
            File.WriteAllLines("./ToDoData/_.txt", inputStrings);
        }
    }
}
