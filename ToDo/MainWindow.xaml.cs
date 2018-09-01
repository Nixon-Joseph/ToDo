using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ToDo.Controls;

namespace ToDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ToDoInput> Inputs { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var input = new ToDoInput();

            Inputs = new List<ToDoInput>() { input };

            InputStack.Children.Add(input);

            input.PreviewKeyDown += Input_KeyDown;
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
        }

        private void AddNewInput(ToDoInput sender, int index)
        {
            var newInput = new ToDoInput(sender.TabSize);
            newInput.PreviewKeyDown += Input_KeyDown;
            InputStack.Children.Insert(index + 1, newInput);
            Inputs.Insert(index + 1, newInput);
            newInput.Loaded += (_sender, _e) => { newInput.Focus(); };
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
    }
}
