
Imports System.IO
Imports System.ServiceProcess
Imports System.Threading
Imports System.Diagnostics


Module Module1
    Dim ServiceError As Integer = 0
    Const ApplicationName As String = "MaintModeOn"
    Const ApplicationVersion As String = "v0.5 02/18/2020"


    Sub Main()

        If My.User.IsInRole(ApplicationServices.BuiltInRole.Administrator) Then
            Console.WriteLine("ERROR: Must be run as Administrator")
            Environment.Exit(244)
        End If

        'Console.WriteLine("Starting " + ApplicationName + " " + ApplicationVersion)
        pWriteStatus("Starting " + ApplicationName + " " + ApplicationVersion, EventLogEntryType.Information)
        Dim flag As Boolean = Not File.Exists("c:\batch\service_list_mm_on.txt")
        If flag Then
            ServiceError = 255
            'Console.WriteLine("ERROR c:\batch\service_list_mm_on.txt does not exist")
            pWriteStatus("ERROR c:\batch\service_list_mm_on.txt does not exist", EventLogEntryType.Error)
        Else
            For Each myService As String In File.ReadAllLines("c:\batch\service_list_mm_on.txt")
                Dim flag2 As Boolean = myService.Length > 1
                If flag2 Then
                    'Console.WriteLine("Stopping:" + myService)
                    pWriteStatus("Stopping:" + myService, EventLogEntryType.Information)
                    pStopService(myService)
                End If
            Next
            pWriteStatus("Complete with Exit Code " + ServiceError.ToString, EventLogEntryType.Information)
            Environment.Exit(ServiceError)
        End If

    End Sub


    Private Sub pStopService(ServiceName As String)
        Dim serviceController As ServiceController = New ServiceController(ServiceName)
        Try
            Process.Start("sc", "config """ + ServiceName + """ start= disabled")
        Catch ex As Exception
            'Console.WriteLine("ERROR " + ex.Message + " when disabling service " + ServiceName)
            pWriteStatus("ERROR " + ex.Message + " when disabling service " + ServiceName, EventLogEntryType.Error)
            ServiceError = 1
            Return
        End Try
        Thread.Sleep(5000)
        Try
            Dim flag As Boolean = serviceController.Status.Equals(ServiceControllerStatus.Stopped)
            If flag Then
                'Console.WriteLine(ServiceName + " is already stopped")
                pWriteStatus(ServiceName + " is already stopped", EventLogEntryType.Information)
                Return
            End If
        Catch ex2 As Exception
            'Console.WriteLine("ERROR " + ex2.Message + " when checking " + ServiceName)
            pWriteStatus("ERROR " + ex2.Message + " when checking " + ServiceName, EventLogEntryType.Error)
            ServiceError = 1
            Return
        End Try
        Try
            serviceController.[Stop]()
        Catch ex3 As Exception
            'Console.WriteLine("ERROR " + ex3.Message + " when stopping " + ServiceName)
            pWriteStatus("ERROR " + ex3.Message + " when stopping " + ServiceName, EventLogEntryType.Error)
            ServiceError = 1
            Return
        End Try
        Thread.Sleep(10000)
        serviceController.Refresh()
        Dim num As Integer = 0
        Dim flag3 As Boolean
        Do
            Dim flag2 As Boolean = serviceController.Status.Equals(ServiceControllerStatus.Stopped)
            If flag2 Then
                Return
            End If
            'Console.WriteLine("Wating for " + ServiceName + " to stop")
            pWriteStatus("Wating for " + ServiceName + " to stop", EventLogEntryType.Information)
            Thread.Sleep(60000)
            serviceController.Refresh()
            num += 1
            flag3 = (num > 5)
        Loop While Not flag3
        ServiceError = 1
    End Sub

    Private Sub pWriteStatus(Status As String, Type As EventLogEntryType)
        WriteToEventLog(Status, ApplicationName, Type)
        Console.WriteLine(Status)
    End Sub

    REM http://www.freevbcode.com/ShowCode.asp?ID=2298
    Public Function WriteToEventLog(ByVal Entry As String,
        Optional ByVal AppName As String = ApplicationName,
        Optional ByVal EventType As EventLogEntryType = EventLogEntryType.Information,
        Optional ByVal LogName As String = "Application") As Boolean

        '*************************************************************
        'PURPOSE: Write Entry to Event Log using VB.NET
        'PARAMETERS: Entry - Value to Write
        '            AppName - Name of Client Application. Needed 
        '              because before writing to event log, you must 
        '              have a named EventLog source. 
        '            EventType - Entry Type, from EventLogEntryType 
        '              Structure e.g., EventLogEntryType.Warning, 
        '              EventLogEntryType.Error
        '            LogName: Name of Log (System, Application; 
        '              Security is read-only) If you 
        '              specify a non-existent log, the log will be
        '              created

        'RETURNS:   True if successful, false if not

        'EXAMPLES: 
        '1. Simple Example, Accepting All Defaults
        '    WriteToEventLog "Hello Event Log"

        '2.  Specify EventSource, EventType, and LogName
        '    WriteToEventLog("Danger, Danger, Danger", "MyVbApp", _
        '                      EventLogEntryType.Warning, "System")
        '
        'NOTE:     EventSources are tightly tied to their log. 
        '          So don't use the same source name for different 
        '          logs, and vice versa
        '******************************************************

        Dim objEventLog As New EventLog()

        Try
            'Register the App as an Event Source
            If Not objEventLog.SourceExists(AppName) Then

                objEventLog.CreateEventSource(AppName, LogName)
            End If

            objEventLog.Source = AppName

            'WriteEntry is overloaded; this is one
            'of 10 ways to call it
            objEventLog.WriteEntry(Entry, EventType)
            Return True
        Catch Ex As Exception
            Console.WriteLine("Error Loggin Stuff: " + Ex.Message)
            Console.WriteLine("Try running as Admin (Elevated)")
            Return False

        End Try

    End Function

End Module
