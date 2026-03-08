using System.Runtime.CompilerServices;
using AI___model.Console.Views;

namespace AI___model.Console;

//TODO: remove static

public static class Console
{
    private static IConsoleView _view = new PreProcessingView();
    
    public static Task Run()
    {
        _view.ShowOnAppear();
        
        while (true)
        {
            string input = System.Console.ReadLine() ?? "";
            _view.HandleInput(input);
        }
    }

    public static void ChangeView(IConsoleView view)
    {
        _view = view;
    }
}