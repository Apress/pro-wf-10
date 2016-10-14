<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" 
    Inherits="WebInvokerHost.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" 
            Text="Enter Test Number:"></asp:Label>
        <br />
        <asp:TextBox ID="TextBox1" runat="server" 
            Width="84px">1001</asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" 
            OnClick="Button1_Click" Text="Run Workflow" />
        <br />
        <asp:Label ID="Label2" runat="server"></asp:Label>
        <br />
        <br />
        <asp:TextBox ID="TextBox2" runat="server" Height="128px" 
            ReadOnly="True" TextMode="MultiLine"
            Width="456px" Wrap="False"></asp:TextBox>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
