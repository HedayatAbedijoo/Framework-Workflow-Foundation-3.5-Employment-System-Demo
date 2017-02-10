<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CheckDocuments.aspx.cs" Inherits="AspHostApplication.CheckDocuments" %>

<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <table style="width: 400px">
        <tr>
            <td align="left" colspan="2" style="text-align: center">
                <span style="color: #0066CC"><i>
                <b style="font-size: x-large">Check Documents</b></i></span>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2" style="text-align: center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="Label1" runat="server" Text="lblName"></asp:Label>
            </td>
            <td align="right">
                EmpolyeeName
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtOccupation" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </td>
            <td align="right">
                Occupation
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtYearExperiences" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </td>
            <td align="right">
                Year Experiences
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtInteresting" runat="server" Height="94px" OnTextChanged="TextBox1_TextChanged"
                    Style="margin-right: 0px" TextMode="MultiLine"></asp:TextBox>
            </td>
            <td align="right" valign="top">
                Interesting
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:CheckBox ID="chkIsDocumentOk" runat="server" Text=" " />
            </td>
            <td align="right">
                IsDocumentOk
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:Button ID="btnSave" runat="server" Text="ذخیره" onclick="btnSave_Click" 
                    Width="154px" />
                &nbsp; &nbsp;
            </td>
        </tr>
        <tr>
            <td align="left" style="height: 23px">
                &nbsp;
            </td>
            <td align="right" style="height: 23px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:DropDownList ID="DropDownList1" runat="server">
                </asp:DropDownList>
                <asp:Button ID="btnGoNext" runat="server" onclick="btnGoNext_Click" 
                    Text="اقدام" Width="126px" />
                &nbsp;
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
