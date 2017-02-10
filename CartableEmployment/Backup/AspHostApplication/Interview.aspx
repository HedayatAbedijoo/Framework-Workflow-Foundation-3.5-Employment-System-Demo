<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Interview.aspx.cs" Inherits="AspHostApplication.Interview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" style="font-size: x-large; font-weight: 700; font-style: italic">
                <span style="color: #0066CC">Interview</span>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: x-large; font-weight: 700; font-style: italic">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                EmployeeName
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtIQScore" runat="server"></asp:TextBox>
            </td>
            <td>
                IQScore
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtLanguage" runat="server"></asp:TextBox>
            </td>
            <td>
                Language
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtTechnicalScore" runat="server"></asp:TextBox>
            </td>
            <td>
                Technical Score
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtPersonalScore" runat="server"></asp:TextBox>
            </td>
            <td>
                Personal Score
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="BtnOk" runat="server" Text="ذخیره" Width="128px" OnClick="BtnOk_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnGoNext" runat="server" Text="تایید" Width="128px" 
                    onclick="btnGoNext_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
