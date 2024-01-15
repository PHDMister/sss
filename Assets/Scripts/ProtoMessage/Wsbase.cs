// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: wsbase.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Base {

  /// <summary>Holder for reflection information generated from wsbase.proto</summary>
  public static partial class WsbaseReflection {

    #region Descriptor
    /// <summary>File descriptor for wsbase.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static WsbaseReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cgx3c2Jhc2UucHJvdG8SBGJhc2UiYQoGV1NEYXRhEgoKAmlkGAEgASgEEhQK",
            "DG1lc3NhZ2VfdHlwZRgCIAEoDRITCgtjbGllbnRfY29kZRgDIAEoDRISCgpl",
            "cnJvcl9jb2RlGAQgASgNEgwKBGRhdGEYBSABKAwiOwoIVXNlckluZm8SDAoE",
            "bmFtZRgBIAEoCRIPCgd1c2VyX2lkGAIgASgEEhAKCGhlYWRfdXJsGAMgASgJ",
            "QgZaBGJhc2ViBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Base.WSData), global::Base.WSData.Parser, new[]{ "Id", "MessageType", "ClientCode", "ErrorCode", "Data" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Base.UserInfo), global::Base.UserInfo.Parser, new[]{ "Name", "UserId", "HeadUrl" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class WSData : pb::IMessage<WSData> {
    private static readonly pb::MessageParser<WSData> _parser = new pb::MessageParser<WSData>(() => new WSData());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<WSData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Base.WsbaseReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WSData() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WSData(WSData other) : this() {
      id_ = other.id_;
      messageType_ = other.messageType_;
      clientCode_ = other.clientCode_;
      errorCode_ = other.errorCode_;
      data_ = other.data_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WSData Clone() {
      return new WSData(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 消息id, 自增
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "message_type" field.</summary>
    public const int MessageTypeFieldNumber = 2;
    private uint messageType_;
    /// <summary>
    /// 消息类型
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint MessageType {
      get { return messageType_; }
      set {
        messageType_ = value;
      }
    }

    /// <summary>Field number for the "client_code" field.</summary>
    public const int ClientCodeFieldNumber = 3;
    private uint clientCode_;
    /// <summary>
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ClientCode {
      get { return clientCode_; }
      set {
        clientCode_ = value;
      }
    }

    /// <summary>Field number for the "error_code" field.</summary>
    public const int ErrorCodeFieldNumber = 4;
    private uint errorCode_;
    /// <summary>
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ErrorCode {
      get { return errorCode_; }
      set {
        errorCode_ = value;
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 5;
    private pb::ByteString data_ = pb::ByteString.Empty;
    /// <summary>
    /// 业务数据
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Data {
      get { return data_; }
      set {
        data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as WSData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(WSData other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (MessageType != other.MessageType) return false;
      if (ClientCode != other.ClientCode) return false;
      if (ErrorCode != other.ErrorCode) return false;
      if (Data != other.Data) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0UL) hash ^= Id.GetHashCode();
      if (MessageType != 0) hash ^= MessageType.GetHashCode();
      if (ClientCode != 0) hash ^= ClientCode.GetHashCode();
      if (ErrorCode != 0) hash ^= ErrorCode.GetHashCode();
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
      if (MessageType != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(MessageType);
      }
      if (ClientCode != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(ClientCode);
      }
      if (ErrorCode != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(ErrorCode);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(42);
        output.WriteBytes(Data);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      if (MessageType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(MessageType);
      }
      if (ClientCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ClientCode);
      }
      if (ErrorCode != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ErrorCode);
      }
      if (Data.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Data);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(WSData other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0UL) {
        Id = other.Id;
      }
      if (other.MessageType != 0) {
        MessageType = other.MessageType;
      }
      if (other.ClientCode != 0) {
        ClientCode = other.ClientCode;
      }
      if (other.ErrorCode != 0) {
        ErrorCode = other.ErrorCode;
      }
      if (other.Data.Length != 0) {
        Data = other.Data;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
          case 16: {
            MessageType = input.ReadUInt32();
            break;
          }
          case 24: {
            ClientCode = input.ReadUInt32();
            break;
          }
          case 32: {
            ErrorCode = input.ReadUInt32();
            break;
          }
          case 42: {
            Data = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  public sealed partial class UserInfo : pb::IMessage<UserInfo> {
    private static readonly pb::MessageParser<UserInfo> _parser = new pb::MessageParser<UserInfo>(() => new UserInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<UserInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Base.WsbaseReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UserInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UserInfo(UserInfo other) : this() {
      name_ = other.name_;
      userId_ = other.userId_;
      headUrl_ = other.headUrl_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UserInfo Clone() {
      return new UserInfo(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "user_id" field.</summary>
    public const int UserIdFieldNumber = 2;
    private ulong userId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong UserId {
      get { return userId_; }
      set {
        userId_ = value;
      }
    }

    /// <summary>Field number for the "head_url" field.</summary>
    public const int HeadUrlFieldNumber = 3;
    private string headUrl_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string HeadUrl {
      get { return headUrl_; }
      set {
        headUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as UserInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(UserInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (UserId != other.UserId) return false;
      if (HeadUrl != other.HeadUrl) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (UserId != 0UL) hash ^= UserId.GetHashCode();
      if (HeadUrl.Length != 0) hash ^= HeadUrl.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (UserId != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(UserId);
      }
      if (HeadUrl.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(HeadUrl);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (UserId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(UserId);
      }
      if (HeadUrl.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(HeadUrl);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(UserInfo other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.UserId != 0UL) {
        UserId = other.UserId;
      }
      if (other.HeadUrl.Length != 0) {
        HeadUrl = other.HeadUrl;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 16: {
            UserId = input.ReadUInt64();
            break;
          }
          case 26: {
            HeadUrl = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
