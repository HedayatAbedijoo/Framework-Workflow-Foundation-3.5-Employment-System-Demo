<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master"
    CodeBehind="RegisterApplicant.aspx.cs" Inherits="AspHostApplication.RegisterApplicant" %>

<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <table style="width: 400px;">
        <tr>
            <td colspan="2" style="font-size: x-large; color: #0066CC;">
                <i><b>Register Applicant </b></i>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: x-large; color: #0066CC;">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtId" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                ID
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                FirstName
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                LastName
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtAge" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                Age
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtIdentityNo" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                IdentityNo
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtGener" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                Gender
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtDegree" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                Degree
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:TextBox ID="txtRegisterDate" runat="server"></asp:TextBox>
            </td>
            <td align="right">
                RegisterDate
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:CheckBox ID="chkOk" runat="server" Text=" " />
            </td>
            <td align="right">
                IsRegisterOK
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" Text="ایجاد متقاضی" Width="126px" OnClick="Button1_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnGoNext" runat="server" Text="اقدام" OnClick="btnGoNext_Click"
                    Width="94px" />
                <asp:DropDownList ID="DropDownList1" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>
