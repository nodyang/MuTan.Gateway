'--------------------------------------------------   
'   
'   �������   
'   
'   namespace: Program
'   author: ľ̿(WoodCoal)
'   homepage: http://www.woodcoal.cn/   
'   memo: �������
'   release: 2020-07-20
'   
'-------------------------------------------------- 

Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports System.Threading

Friend Module Program

	''' <summary>�ͻ��˲�������</summary>
	Friend xDatabase As Utils.Database

	''' <summary>JWT��������</summary>
	Friend xJWT As Utils.JWT

	''' <summary>��Ȩ��֤����</summary>
	Friend xAuthorization As Utils.Authorization

	''' <summary>�������</summary>
	Friend xCache As Utils.Cache

	''' <summary>��������</summary>
	Friend xConfig As Utils.Config

	''' <summary>ϵͳ��Ϣͳ��</summary>
	Friend xSystemInforamtion As Counter.System

	''' <summary>��������ԱȨ������</summary>
	Friend Const Super As String = "_.SUPER._"

	''' <summary>�Զ���Ⲣ��ȡʵ��·��</summary>
	''' <param name="source"></param>
	''' <param name="tryCreate">�Ƿ��Դ�����·�����ϼ�Ŀ¼���磺d:\a\b\c True ���Զ����� d:\a\b ��Ŀ¼</param>
	''' <param name="isFolder">��ǰ��ȡ����Ŀ¼�����ļ���ַ���Ա㽨����Ӧ��Ŀ¼</param>
	Public Function Root(ByVal source As String, Optional tryCreate As Boolean = False, Optional isFolder As Boolean = False) As String
		Dim R = AppDomain.CurrentDomain.BaseDirectory

		If Not String.IsNullOrWhiteSpace(source) Then
			Dim sp = IO.Path.DirectorySeparatorChar

			source = source.Replace("\", sp)
			source = source.Replace("/", sp)

			If R.Substring(1, 1) = ":" AndAlso source.StartsWith(sp) Then
				R = R.Substring(0, 2) & source
			Else
				R = IO.Path.Combine(R, source)
			End If

			If tryCreate Then
				Dim f = If(isFolder, R, IO.Path.GetDirectoryName(R))
				If Not IO.Directory.Exists(f) Then IO.Directory.CreateDirectory(f)
			End If
		End If

		Return R
	End Function


	Friend Sub Main()
		Dim builder = New HostBuilder().ConfigureServices(Sub(hostContext, services) services.AddHostedService(Of HttpServerHost))
		builder.Build().Run()
	End Sub

End Module

Friend Class HttpServerHost
	Implements IHostedService

	Private g As Bumblebee.Gateway

	Public Function StartAsync(cancellationToken As CancellationToken) As Task Implements IHostedService.StartAsync
		' �������ݿ����
		xDatabase = New Utils.Database

		' ���ز�������
		xConfig = Utils.Config.Default

		' ����JWT��������
		xJWT = New Utils.JWT(xConfig.TOKEN_ISSUER, xConfig.TOKEN_AUDIECNCE, xConfig.TOKEN_KEY)

		' ��Ȩ��֤����
		xAuthorization = New Utils.Authorization

		' �������
		xCache = New Utils.Cache

		' ���ض���
		g = New Bumblebee.Gateway
		g.HttpOptions(Sub(o)
						  o.Host = xConfig.SYS_HOST
						  o.Port = xConfig.SYS_PORT
						  o.SSL = xConfig.SYS_SSL
						  o.SSLPort = xConfig.SYS_SSL_PORT
						  o.CertificateFile = xConfig.SYS_SSL_FILE
						  o.CertificatePassword = xConfig.SYS_SSL_FILE

						  o.LogLevel = xConfig.SYS_LOG_LEVEL
						  o.LogToConsole = xConfig.SYS_LOG_CONSOLE
						  o.WriteLog = xConfig.SYS_LOG_SAVE
						  o.OutputStackTrace = xConfig.SYS_LOG_STACKTRACE
					  End Sub)
		g.StatisticsEnabled = xConfig.SYS_STATISTICS
		g.Open()
		g.LoadPlugin(Me.GetType.Assembly)

		'Dim Files = IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
		'If Files?.Length > 0 Then
		'    For Each File In Files
		'        Try
		'            Me.g.LoadPlugin(Assembly.LoadFile(File))
		'        Catch ex As Exception
		'        End Try
		'    Next
		'End If

		' ϵͳ��Ϣͳ��
		xSystemInforamtion = New Counter.System(g)

		Return Task.CompletedTask
	End Function

	Public Function StopAsync(cancellationToken As CancellationToken) As Task Implements IHostedService.StopAsync
		xSystemInforamtion.Dispose()
		xDatabase.Dispose()

		g.Dispose()
		Return Task.CompletedTask
	End Function

End Class