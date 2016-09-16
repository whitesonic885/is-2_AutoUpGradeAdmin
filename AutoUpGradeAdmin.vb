Imports System.IO

Module AutoUpGradeAdmin

'    Private Const constUsage As String = vbCrLf & "指定されたアプリケーションを起動します" & vbCrLf & _
'                                         vbCrLf & _
'                                         "AutoUpGradeMainte.exe 実行ファイル名 f1 f2 f3" & vbCrLf & _
'                                         vbCrLf & _
'                                         "実行ファイル名" & vbTab & "実行可能ファイルをフルパス又はURLで指定します" & vbCrLf & _
'                                         "f1" & vbTab & vbTab & "0 ダウンロードしない(default)" & vbTab & "1 ダウンロードを行う" & vbCrLf & _
'                                         "  " & vbTab & vbTab & "9 ダウンロードのみで起動しない(f2パラメータは無効になります)" & vbCrLf & _
'                                         "f2" & vbTab & vbTab & "0 ローカル起動しない(default)" & vbTab & "1 ローカル起動を行う" & vbCrLf & _
'                                         "f3" & vbTab & vbTab & "0 ログを追記する(default)    " & vbTab & "1 ログを消去してから記録を行う" & vbCrLf


    <STAThread()> _
    Sub Main()
        Dim myAutoUpGrade As AutoUpGradeUtility
        Dim myRet As Boolean
        Dim myFolderName As String
        Dim myFileName As String
        Dim myDownload As Boolean = True
        Dim myLocalExecute As Boolean = True
        Dim myLogClear As Boolean = True
        Dim myDownloadOnly As Boolean = False
        Dim cmdArgs() As String = Environment.GetCommandLineArgs()
        Dim cmdPara(1) As String

        '// ADD 2005.05.31 東都）伊賀 ２重起動防止 START
        ' Mutex の新しいインスタンスを生成 (Mutex の名前にアセンブリ名を付ける)
        Dim hMutex As New System.Threading.Mutex(False, "is2_AutoUpGradeAdmin")

        ' Mutex のシグナルを受信できるかどうか判断
        If hMutex.WaitOne(0, False) = True Then
            GC.KeepAlive(hMutex)         ' hMutex をガベージ コレクション対象から除外する
        Else
            Environment.Exit(-1)         ' AutoUpGradeを終了する
            Exit Sub
        End If
        '// ADD 2005.05.31 東都）伊賀 ２重起動防止 END

        'パラメーターのチェック
        If cmdArgs.Length > 2 Then
            ShowMessage("パラメーターが多すぎます")
            '// ADD 2005.05.31 東都）伊賀 ２重起動防止 START
            ' Mutex を開放する
            hMutex.ReleaseMutex()
            hMutex.Close()
            '// ADD 2005.05.31 東都）伊賀 ２重起動防止 END
            Environment.Exit(-1)
            Exit Sub
        End If
        If cmdArgs.Length = 2 Then
            cmdPara(0) = " " & cmdArgs(1)
        Else
            cmdPara(0) = ""
        End If

        '対象フォルダ名
        '// MOD 2005.06.24 東都）伊賀 httpsとhttpの切り替え START
        'myFolderName = "http://wwwis2.fukutsu.co.jp/is2/ReleaseAdmin/"
        myFolderName = "https://wwwis2.fukutsu.co.jp/is2/ReleaseAdmin/"
        '// MOD 2005.06.24 東都）伊賀 httpsとhttpの切り替え END
        myFileName = "is2AdminClient.exe"

        '対象のファイルをダウンロードする
        '対象フォルダにあるファイル一覧の取得
        myAutoUpGrade = New AutoUpGradeUtility
        '// ADD 2005.05.31 東都）伊賀 CopyAutoUpGrade対応 START
        myAutoUpGrade.gClientMutex = "is2_AutoUpGradeAdmin"
        '// ADD 2005.05.31 東都）伊賀 CopyAutoUpGrade対応 END
        myRet = myAutoUpGrade.GetVersion(myFolderName, myFileName, myDownload, myDownloadOnly, myLocalExecute, myLogClear)

        'アプリケーションの起動
        '        myRet = myAutoUpGrade.Startup(myFolderName, myDownload, myDownloadOnly, myLocalExecute, myLogClear)

        If myRet = False Then
            ShowMessage("サーバーとの通信に失敗しました" & vbCrLf & "ＬＡＮケーブルやネットワーク設定等を確認してください")
            '// ADD 2005.05.31 東都）伊賀 ２重起動防止 START
            ' Mutex を開放する
            hMutex.ReleaseMutex()
            hMutex.Close()
            '// ADD 2005.05.31 東都）伊賀 ２重起動防止 END
            Environment.Exit(-1)
            Exit Sub
        End If

        'アプリケーションの起動
        myRet = myAutoUpGrade.ExecApp(myFileName, cmdPara, myLocalExecute)

        '// ADD 2005.05.31 東都）伊賀 ２重起動防止 START
        ' Mutex を開放する
        hMutex.ReleaseMutex()
        hMutex.Close()
        '// ADD 2005.05.31 東都）伊賀 ２重起動防止 END

        Environment.Exit(0)
    End Sub

    '
    '   メッセージを表示
    '
    Private Sub ShowMessage(ByVal addMessage As String)
        Windows.Forms.MessageBox.Show(addMessage, _
                                      "is2Admin", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Exclamation)
    End Sub

End Module
