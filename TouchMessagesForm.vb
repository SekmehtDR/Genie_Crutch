Imports System.Windows.Forms
Imports System.Drawing

Public Class TouchMessagesForm
    Inherits System.Windows.Forms.Form

    Private WithEvents ListBoxPatterns As New ListBox()
    Private WithEvents TextBoxPattern As New TextBox()
    Private WithEvents ButtonAdd As New Button()
    Private WithEvents ButtonUpdate As New Button()
    Private WithEvents ButtonRemove As New Button()
    Private WithEvents ButtonSetActive As New Button()
    Private WithEvents ButtonClose As New Button()

    Public Sub New()
        InitializeLayout()
        RefreshList()
    End Sub

    Private Sub InitializeLayout()
        Me.Text = "Touch Message Patterns"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(460, 310)
        Me.ShowInTaskbar = False

        Dim labelTitle As New Label()
        labelTitle.Text = "Saved patterns — use {name} where the patient's name appears:"
        labelTitle.Location = New Point(8, 8)
        labelTitle.Size = New Size(444, 16)
        Me.Controls.Add(labelTitle)

        ListBoxPatterns.Location = New Point(8, 28)
        ListBoxPatterns.Size = New Size(444, 150)
        ListBoxPatterns.Font = New Font("Courier New", 8.25!)
        Me.Controls.Add(ListBoxPatterns)

        Dim labelInput As New Label()
        labelInput.Text = "Pattern:"
        labelInput.Location = New Point(8, 188)
        labelInput.Size = New Size(50, 16)
        Me.Controls.Add(labelInput)

        TextBoxPattern.Location = New Point(62, 185)
        TextBoxPattern.Size = New Size(390, 20)
        Me.Controls.Add(TextBoxPattern)

        ButtonAdd.Text = "Add New"
        ButtonAdd.Location = New Point(8, 214)
        ButtonAdd.Size = New Size(80, 23)
        Me.Controls.Add(ButtonAdd)

        ButtonUpdate.Text = "Update"
        ButtonUpdate.Location = New Point(96, 214)
        ButtonUpdate.Size = New Size(80, 23)
        Me.Controls.Add(ButtonUpdate)

        ButtonSetActive.Text = "Set Active"
        ButtonSetActive.Location = New Point(184, 214)
        ButtonSetActive.Size = New Size(80, 23)
        Me.Controls.Add(ButtonSetActive)

        ButtonRemove.Text = "Remove"
        ButtonRemove.Location = New Point(272, 214)
        ButtonRemove.Size = New Size(80, 23)
        Me.Controls.Add(ButtonRemove)

        Dim labelNote As New Label()
        labelNote.Text = "Active pattern is marked with [*]. Select a pattern to edit or set it active."
        labelNote.Location = New Point(8, 248)
        labelNote.Size = New Size(444, 16)
        Me.Controls.Add(labelNote)

        ButtonClose.Text = "Close"
        ButtonClose.Location = New Point(377, 277)
        ButtonClose.Size = New Size(75, 23)
        Me.Controls.Add(ButtonClose)
    End Sub

    Private Sub RefreshList()
        Dim savedIndex As Integer = ListBoxPatterns.SelectedIndex
        ListBoxPatterns.Items.Clear()
        For i As Integer = 0 To Crutch.m_TouchPatterns.Count - 1
            Dim prefix As String = If(i = Crutch.m_ActivePatternIndex, "[*] ", "[ ] ")
            ListBoxPatterns.Items.Add(prefix & Crutch.m_TouchPatterns(i))
        Next
        If savedIndex >= 0 AndAlso savedIndex < ListBoxPatterns.Items.Count Then
            ListBoxPatterns.SelectedIndex = savedIndex
        End If
    End Sub

    Private Sub ListBoxPatterns_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBoxPatterns.SelectedIndexChanged
        Dim idx As Integer = ListBoxPatterns.SelectedIndex
        If idx >= 0 AndAlso idx < Crutch.m_TouchPatterns.Count Then
            TextBoxPattern.Text = Crutch.m_TouchPatterns(idx)
        End If
    End Sub

    Private Function ValidatePattern(ByVal pattern As String) As Boolean
        If pattern.Trim().Length = 0 Then
            MessageBox.Show("Enter a pattern first.", "Crutch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If Not pattern.Contains("{name}") Then
            MessageBox.Show("Pattern must contain {name} to identify the patient's name.", "Crutch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Sub ButtonAdd_Click(sender As Object, e As EventArgs) Handles ButtonAdd.Click
        Dim pattern As String = TextBoxPattern.Text.Trim()
        If Not ValidatePattern(pattern) Then Return
        Crutch.m_TouchPatterns.Add(pattern)
        RefreshList()
        ListBoxPatterns.SelectedIndex = Crutch.m_TouchPatterns.Count - 1
    End Sub

    Private Sub ButtonUpdate_Click(sender As Object, e As EventArgs) Handles ButtonUpdate.Click
        Dim idx As Integer = ListBoxPatterns.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("Select a pattern to update.", "Crutch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim pattern As String = TextBoxPattern.Text.Trim()
        If Not ValidatePattern(pattern) Then Return
        Crutch.m_TouchPatterns(idx) = pattern
        If idx = Crutch.m_ActivePatternIndex Then
            Crutch.RebuildActiveRegex()
        End If
        RefreshList()
    End Sub

    Private Sub ButtonSetActive_Click(sender As Object, e As EventArgs) Handles ButtonSetActive.Click
        Dim idx As Integer = ListBoxPatterns.SelectedIndex
        If idx < 0 Then
            MessageBox.Show("Select a pattern to activate.", "Crutch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Crutch.m_ActivePatternIndex = idx
        Crutch.RebuildActiveRegex()
        RefreshList()
    End Sub

    Private Sub ButtonRemove_Click(sender As Object, e As EventArgs) Handles ButtonRemove.Click
        Dim idx As Integer = ListBoxPatterns.SelectedIndex
        If idx < 0 Then Return
        If Crutch.m_TouchPatterns.Count <= 1 Then
            MessageBox.Show("At least one pattern must remain.", "Crutch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Crutch.m_TouchPatterns.RemoveAt(idx)
        If Crutch.m_ActivePatternIndex >= Crutch.m_TouchPatterns.Count Then
            Crutch.m_ActivePatternIndex = Crutch.m_TouchPatterns.Count - 1
        End If
        Crutch.RebuildActiveRegex()
        RefreshList()
    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click
        Me.Close()
    End Sub

End Class
