using BinaryFileManager.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using System.Windows.Threading;

namespace BinaryFileManager;

public partial class App : Application
{
    private ILogger _logger;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;

        _logger = FileLoggerFactory.GetLogger();
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Obsługa wyjątków nieobsłużonych w głównym wątku (zdarzenie globalne)
        Exception exception = e.ExceptionObject as Exception;
        HandleException(exception);
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Obsługa wyjątków nieobsłużonych w głównym wątku UI
        HandleException(e.Exception);
        e.Handled = true; // Oznacza, że wyjątek został obsłużony
    }

    private void HandleException(Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        MessageBox.Show("Wystąpił błąd. Aplikacja zostanie zamknięta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);

        Shutdown();
    }
}