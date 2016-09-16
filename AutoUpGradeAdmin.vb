Imports System.IO

Module AutoUpGradeAdmin

'    Private Const constUsage As String = vbCrLf & "�w�肳�ꂽ�A�v���P�[�V�������N�����܂�" & vbCrLf & _
'                                         vbCrLf & _
'                                         "AutoUpGradeMainte.exe ���s�t�@�C���� f1 f2 f3" & vbCrLf & _
'                                         vbCrLf & _
'                                         "���s�t�@�C����" & vbTab & "���s�\�t�@�C�����t���p�X����URL�Ŏw�肵�܂�" & vbCrLf & _
'                                         "f1" & vbTab & vbTab & "0 �_�E�����[�h���Ȃ�(default)" & vbTab & "1 �_�E�����[�h���s��" & vbCrLf & _
'                                         "  " & vbTab & vbTab & "9 �_�E�����[�h�݂̂ŋN�����Ȃ�(f2�p�����[�^�͖����ɂȂ�܂�)" & vbCrLf & _
'                                         "f2" & vbTab & vbTab & "0 ���[�J���N�����Ȃ�(default)" & vbTab & "1 ���[�J���N�����s��" & vbCrLf & _
'                                         "f3" & vbTab & vbTab & "0 ���O��ǋL����(default)    " & vbTab & "1 ���O���������Ă���L�^���s��" & vbCrLf


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

        '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ START
        ' Mutex �̐V�����C���X�^���X�𐶐� (Mutex �̖��O�ɃA�Z���u������t����)
        Dim hMutex As New System.Threading.Mutex(False, "is2_AutoUpGradeAdmin")

        ' Mutex �̃V�O�i������M�ł��邩�ǂ������f
        If hMutex.WaitOne(0, False) = True Then
            GC.KeepAlive(hMutex)         ' hMutex ���K�x�[�W �R���N�V�����Ώۂ��珜�O����
        Else
            Environment.Exit(-1)         ' AutoUpGrade���I������
            Exit Sub
        End If
        '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ END

        '�p�����[�^�[�̃`�F�b�N
        If cmdArgs.Length > 2 Then
            ShowMessage("�p�����[�^�[���������܂�")
            '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ START
            ' Mutex ���J������
            hMutex.ReleaseMutex()
            hMutex.Close()
            '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ END
            Environment.Exit(-1)
            Exit Sub
        End If
        If cmdArgs.Length = 2 Then
            cmdPara(0) = " " & cmdArgs(1)
        Else
            cmdPara(0) = ""
        End If

        '�Ώۃt�H���_��
        '// MOD 2005.06.24 ���s�j�ɉ� https��http�̐؂�ւ� START
        'myFolderName = "http://wwwis2.fukutsu.co.jp/is2/ReleaseAdmin/"
        myFolderName = "https://wwwis2.fukutsu.co.jp/is2/ReleaseAdmin/"
        '// MOD 2005.06.24 ���s�j�ɉ� https��http�̐؂�ւ� END
        myFileName = "is2AdminClient.exe"

        '�Ώۂ̃t�@�C�����_�E�����[�h����
        '�Ώۃt�H���_�ɂ���t�@�C���ꗗ�̎擾
        myAutoUpGrade = New AutoUpGradeUtility
        '// ADD 2005.05.31 ���s�j�ɉ� CopyAutoUpGrade�Ή� START
        myAutoUpGrade.gClientMutex = "is2_AutoUpGradeAdmin"
        '// ADD 2005.05.31 ���s�j�ɉ� CopyAutoUpGrade�Ή� END
        myRet = myAutoUpGrade.GetVersion(myFolderName, myFileName, myDownload, myDownloadOnly, myLocalExecute, myLogClear)

        '�A�v���P�[�V�����̋N��
        '        myRet = myAutoUpGrade.Startup(myFolderName, myDownload, myDownloadOnly, myLocalExecute, myLogClear)

        If myRet = False Then
            ShowMessage("�T�[�o�[�Ƃ̒ʐM�Ɏ��s���܂���" & vbCrLf & "�k�`�m�P�[�u����l�b�g���[�N�ݒ蓙���m�F���Ă�������")
            '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ START
            ' Mutex ���J������
            hMutex.ReleaseMutex()
            hMutex.Close()
            '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ END
            Environment.Exit(-1)
            Exit Sub
        End If

        '�A�v���P�[�V�����̋N��
        myRet = myAutoUpGrade.ExecApp(myFileName, cmdPara, myLocalExecute)

        '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ START
        ' Mutex ���J������
        hMutex.ReleaseMutex()
        hMutex.Close()
        '// ADD 2005.05.31 ���s�j�ɉ� �Q�d�N���h�~ END

        Environment.Exit(0)
    End Sub

    '
    '   ���b�Z�[�W��\��
    '
    Private Sub ShowMessage(ByVal addMessage As String)
        Windows.Forms.MessageBox.Show(addMessage, _
                                      "is2Admin", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Exclamation)
    End Sub

End Module
