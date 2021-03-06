﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30128.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkflowClient.ServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OrderProcessingRequest", Namespace="http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    [System.SerializableAttribute()]
    public partial class OrderProcessingRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreditCardField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreditCardExpirationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustomerAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustomerEmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustomerNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<WorkflowClient.ServiceReference.Item> ItemsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal TotalAmountField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreditCard {
            get {
                return this.CreditCardField;
            }
            set {
                if ((object.ReferenceEquals(this.CreditCardField, value) != true)) {
                    this.CreditCardField = value;
                    this.RaisePropertyChanged("CreditCard");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreditCardExpiration {
            get {
                return this.CreditCardExpirationField;
            }
            set {
                if ((object.ReferenceEquals(this.CreditCardExpirationField, value) != true)) {
                    this.CreditCardExpirationField = value;
                    this.RaisePropertyChanged("CreditCardExpiration");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomerAddress {
            get {
                return this.CustomerAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerAddressField, value) != true)) {
                    this.CustomerAddressField = value;
                    this.RaisePropertyChanged("CustomerAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomerEmail {
            get {
                return this.CustomerEmailField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerEmailField, value) != true)) {
                    this.CustomerEmailField = value;
                    this.RaisePropertyChanged("CustomerEmail");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomerName {
            get {
                return this.CustomerNameField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerNameField, value) != true)) {
                    this.CustomerNameField = value;
                    this.RaisePropertyChanged("CustomerName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<WorkflowClient.ServiceReference.Item> Items {
            get {
                return this.ItemsField;
            }
            set {
                if ((object.ReferenceEquals(this.ItemsField, value) != true)) {
                    this.ItemsField = value;
                    this.RaisePropertyChanged("Items");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal TotalAmount {
            get {
                return this.TotalAmountField;
            }
            set {
                if ((this.TotalAmountField.Equals(value) != true)) {
                    this.TotalAmountField = value;
                    this.RaisePropertyChanged("TotalAmount");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Item", Namespace="http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    [System.SerializableAttribute()]
    public partial class Item : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ItemIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int QuantityField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ItemId {
            get {
                return this.ItemIdField;
            }
            set {
                if ((this.ItemIdField.Equals(value) != true)) {
                    this.ItemIdField = value;
                    this.RaisePropertyChanged("ItemId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OrderProcessingResponse", Namespace="http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    [System.SerializableAttribute()]
    public partial class OrderProcessingResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreditAuthCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessfulField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int OrderIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ShipDateField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreditAuthCode {
            get {
                return this.CreditAuthCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.CreditAuthCodeField, value) != true)) {
                    this.CreditAuthCodeField = value;
                    this.RaisePropertyChanged("CreditAuthCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSuccessful {
            get {
                return this.IsSuccessfulField;
            }
            set {
                if ((this.IsSuccessfulField.Equals(value) != true)) {
                    this.IsSuccessfulField = value;
                    this.RaisePropertyChanged("IsSuccessful");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OrderId {
            get {
                return this.OrderIdField;
            }
            set {
                if ((this.OrderIdField.Equals(value) != true)) {
                    this.OrderIdField = value;
                    this.RaisePropertyChanged("OrderId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ShipDate {
            get {
                return this.ShipDateField;
            }
            set {
                if ((this.ShipDateField.Equals(value) != true)) {
                    this.ShipDateField = value;
                    this.RaisePropertyChanged("ShipDate");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference.IOrderProcessing")]
    public interface IOrderProcessing {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderProcessing/ProcessOrder", ReplyAction="http://tempuri.org/IOrderProcessing/ProcessOrderResponse")]
        WorkflowClient.ServiceReference.ProcessOrderResponse ProcessOrder(WorkflowClient.ServiceReference.ProcessOrderRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ProcessOrderRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://schemas.datacontract.org/2004/07/ServiceLibrary", Order=0)]
        public WorkflowClient.ServiceReference.OrderProcessingRequest OrderProcessingRequest;
        
        public ProcessOrderRequest() {
        }
        
        public ProcessOrderRequest(WorkflowClient.ServiceReference.OrderProcessingRequest OrderProcessingRequest) {
            this.OrderProcessingRequest = OrderProcessingRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ProcessOrderResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://schemas.datacontract.org/2004/07/ServiceLibrary", Order=0)]
        public WorkflowClient.ServiceReference.OrderProcessingResponse OrderProcessingResponse;
        
        public ProcessOrderResponse() {
        }
        
        public ProcessOrderResponse(WorkflowClient.ServiceReference.OrderProcessingResponse OrderProcessingResponse) {
            this.OrderProcessingResponse = OrderProcessingResponse;
        }
    }
}
