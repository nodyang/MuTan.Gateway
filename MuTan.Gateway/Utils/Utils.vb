'--------------------------------------------------   
'   
'   �����չ����   
'   
'   namespace: Utils.Utils
'   author: ľ̿(WoodCoal)
'   homepage: http://www.woodcoal.cn/   
'   memo: �����չ����
'   release: 2020-06-03
'   
'-------------------------------------------------- 

Imports System.Text.RegularExpressions
Imports BeetleX.FastHttpApi
Imports Newtonsoft.Json

Namespace Utils

	''' <summary>�����չ����</summary>
	Public Module Utils

		''' <summary>�û�IP</summary>
		Public ReadOnly Property GetIP(Request As HttpRequest) As String
			Get
				Dim Value = ""

				If Request IsNot Nothing Then
					Value = Request.Header("HTTP_X_FORWARDED_FOR")

					If String.IsNullOrWhiteSpace(Value) Then
						Value = Request.Header("X-Forwarded-For")
						If Not String.IsNullOrEmpty(Value) AndAlso Value.Contains(",") Then Value = Value.Split(",")(0)
					End If
					If String.IsNullOrWhiteSpace(Value) Then Value = Request.RemoteIPAddress
				End If

				Return Value
			End Get
		End Property

		''' <summary>��ȡ Unix ʱ�����ע�� 2038 �����⣬��С��λΪ��</summary>
		Public Function UnixTicks(d As Date) As Integer
			Return (d.ToUniversalTime.Ticks - 621355968000000000) / 10000000
		End Function

		''' <summary>��ȡ Js ʱ�������С��λΪ����</summary>
		Public Function JsTicks(d As Date) As Long
			Return (d.ToUniversalTime.Ticks - 621355968000000000) / 10000
		End Function

		''' <summary>�Ƿ����</summary>
		Public Function Include(Source As String, Search As String) As Boolean
			Dim R = ""

			If Not String.IsNullOrWhiteSpace(Source) AndAlso Not String.IsNullOrWhiteSpace(Search) Then
				If Search.StartsWith("*") AndAlso Search.EndsWith("*") Then


				End If

			End If

			Return R
		End Function

#Region "���¼�"

		''' <summary>���¼�����</summary>
		Public Sub UpdateKey(ByRef Key As String)
			If Not String.IsNullOrWhiteSpace(Key) Then
				Key = Key.Trim.ToLower
			Else
				Key = ""
			End If
		End Sub

		''' <summary>�������ƣ���ĸ��ͷ��24���ַ���������ĸ���ֺ��ߣ�</summary>
		Public Sub UpdateName(ByRef Name As String)
			Dim Ret = ""

			If Not String.IsNullOrWhiteSpace(Name) Then
				Name = Name.Trim
				If Regex.IsMatch(Name, "^[a-zA-Z]{1,1}[0-9a-zA-Z_\-]{2,23}$") Then Ret = Name.ToLower
			End If

			Name = Ret
		End Sub

		''' <summary>����ƥ�����ֵ</summary>
		Public Sub UpdateMatchRule(ByRef Rule As String)
			Rule = New Model.Match.Rule(Rule).OriginalString
		End Sub

		''' <summary>����ƥ�����ֵ</summary>
		Public Sub UpdateMatchRules(ByRef Rules As String())
			Dim Ret As New List(Of String)

			If Rules?.Length > 0 Then
				For Each Rule In Rules
					UpdateMatchRule(Rule)
					If Not Ret.Contains(Rule) Then Ret.Add(Rule)
				Next
			End If

			Rules = Ret.ToArray
		End Sub

		''' <summary>�������飬���˿��������ظ�����</summary>
		Public Sub UpdateArray(ByRef Arr As String())
			Dim Ret As New List(Of String)

			If Arr?.Length > 0 Then
				For Each s In Arr
					If Not String.IsNullOrWhiteSpace(s) AndAlso Not Ret.Contains(s, StringComparer.OrdinalIgnoreCase) Then Ret.Add(s)
				Next
			End If

			Arr = Ret.ToArray
		End Sub
#End Region

#Region "JSON �ļ�"

		Public Function SaveJson(ByVal File As String, ByVal Obj As Object, Optional Indented As Boolean = False) As Boolean
			Dim R = False

			If Not String.IsNullOrEmpty(File) Then
				Dim Path = Root(File, True)

				If Obj Is Nothing Then
					IO.File.WriteAllText(Path, "")
				Else
					Try
						Using fs As IO.FileStream = IO.File.Create(File)
							Using Stream As New IO.StreamWriter(fs)
								Dim serializer As New JsonSerializer With {.NullValueHandling = NullValueHandling.Ignore}

								Using writer As New JsonTextWriter(Stream)
									If Indented Then writer.Formatting = Formatting.Indented
									serializer.Serialize(writer, Obj)
								End Using

								R = True
							End Using
						End Using
					Catch ex As Exception
					End Try
				End If
			End If

			Return R
		End Function

		Public Function ReadJson(Of T)(ByVal File As String) As T
			Dim R As Object = Nothing

			If Not String.IsNullOrEmpty(File) Then
				Dim Path = Root(File)
				If IO.File.Exists(Path) Then
					Try
						Using fs As IO.FileStream = IO.File.OpenRead(File)
							Using Stream As New IO.StreamReader(fs)
								Dim serializer As New JsonSerializer With {.NullValueHandling = NullValueHandling.Ignore}

								Using reader As JsonReader = New JsonTextReader(Stream)
									R = serializer.Deserialize(Of T)(reader)
								End Using
							End Using
						End Using
					Catch ex As Exception
					End Try
				End If
			End If

			Return R
		End Function

#End Region

	End Module

End Namespace