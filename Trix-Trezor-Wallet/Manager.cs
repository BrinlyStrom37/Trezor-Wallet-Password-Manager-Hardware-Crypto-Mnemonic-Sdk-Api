using System;
using System.Diagnostics;

namespace TrezorLib
{
    public class Manager
    {
        public event EventHandler Attached;
        public event EventHandler Detached;
        public event EventHandler PinRequest;
        public event EventHandler PassphraseRequest;
        public event EventHandler<PublicKey> PublicKey;
        public event EventHandler<Address> Address;
        public event EventHandler<FailureEventArgs> Failure;

        private Device _device = new Device();
        public bool IsConnected { get { return _device.IsConnected; } }

        public Manager()
        {
            _device.Attached += _device_Attached;
            _device.Detached += _device_Detached;

            _device.Connect();
        }

        #region Events

        private void _device_Attached(object sender, EventArgs e)
        {
            Attached?.Invoke(this, null);
        }

        private void _device_Detached(object sender, EventArgs e)
        {
            Detached?.Invoke(this, null);
        }

        private void OnPinMatrixRequest()
        {
            PinRequest?.Invoke(this, null);
        }

        private void OnPassphraseRequest()
        {
            PassphraseRequest?.Invoke(this, null);
        }

        private void OnReturnPublicKey(PublicKey pubKey)
        {
            PublicKey?.Invoke(this, pubKey);
        }

        private void OnAddress(Address address)
        {
            Address?.Invoke(this, address);
        }

        private void OnFailure(FailureType failureType)
        {
            Failure?.Invoke(this, new FailureEventArgs { Type = failureType } );
        }

        #endregion Events

        #region Public Methods
        public Features Initialize()
        {
            if (!_device.IsConnected) return null;

            Utils.Write(new Initialize(), MessageType.MessageTypeInitialize);
            var result = Read();
            
            return result as Features;
        }

        public void Login()
        {
            if (!_device.IsConnected) return;

            Utils.Write(new SignIdentity(), MessageType.MessageTypeSignIdentity);
            Read();
        }

        public void RequestPublicKey()
        {
            if (!_device.IsConnected) return;

            var path = new GetPublicKey();
            
            Utils.Write(path, MessageType.MessageTypeGetPublicKey);
            Read();            
        }

        public  void RequestAddress()
        {
            var request = new GetAddress();
            Utils.Write(request, MessageType.MessageTypeGetAddress);
            Read();
        }

        public void SignTx()
        {
            var tx = new SignTx();
            
            tx.InputsCount = 2;
            tx.OutputsCount = 2;
            Utils.Write(tx, MessageType.MessageTypeSignTx);
            Read();
        }

        #endregion Public Methods

        public void SendPinMatrixAck(string pin)
        {
            var pinMatrixAck = new PinMatrixAck();
            pinMatrixAck.Pin = pin;

            Utils.Write(pinMatrixAck, MessageType.MessageTypePinMatrixAck);
            Read();
        }
        
        public void SendPassphraseAck(string passphrase)
        {
            var passphraseAck = new PassphraseAck();
            passphraseAck.Passphrase = passphrase;

            Utils.Write(passphrase, MessageType.MessageTypePassphraseAck);
            Read();
        }

        private object Read()
        {
            MessageType msgType;
            var buffer = Utils.Read(out msgType);

            return ParseMessage(msgType, buffer);
        }

        private object ParseMessage(MessageType messageType, byte[] msg)
        {
            Debug.WriteLine("TREZOR Readed: {0}", messageType);

            switch (messageType)
            {
                case MessageType.MessageTypeFailure:
                    var failure = Utils.ProtoDeserialize<Failure>(msg);
                    Debug.WriteLine("TREZOR failure: {0}", (object)failure.Message);
                    OnFailure(failure.Code);
                    break;
                case MessageType.MessageTypeFeatures:
                    var features = Utils.ProtoDeserialize<Features>(msg);
                    Debug.WriteLine("TREZOR: {0}", (object)features.Label);
                    return features;
                case MessageType.MessageTypePinMatrixRequest:
                    var pinRequest = Utils.ProtoDeserialize<PinMatrixRequest>(msg);
                    Debug.WriteLine("TREZOR: Pin requested: {0}", pinRequest.Type);
                    OnPinMatrixRequest();
                    break;
                case MessageType.MessageTypePublicKey:
                    var pubKey = Utils.ProtoDeserialize<PublicKey>(msg);
                    Debug.WriteLine("TREZOR: Pub Key: {0}", (object)pubKey.Xpub);
                    OnReturnPublicKey(pubKey);
                    break;
                case MessageType.MessageTypeButtonRequest:
                    var request = Utils.ProtoDeserialize<ButtonRequest>(msg);
                    Debug.WriteLine("TREZOR: Request: {0}", (object)request.Data);
                    //SendButtonAck();
                    break;
                case MessageType.MessageTypeEntropy:
                    var entropy = Utils.ProtoDeserialize<Entropy>(msg);
                    //_entropy = entropy.entropy;
                    break;
                case MessageType.MessageTypeSuccess:
                    var success = Utils.ProtoDeserialize<Success>(msg);
                    Debug.WriteLine("TREZOR: Success: {0}", (object)success.Message);
                    break;
                case MessageType.MessageTypeEntropyRequest:
                    //SendEntropyAck();
                    break;
                case MessageType.MessageTypePassphraseRequest:
                    Debug.WriteLine("TREZOR: Passphrase requested: {0}");
                    OnPassphraseRequest();
                    break;
                case MessageType.MessageTypeAddress:
                    var address = Utils.ProtoDeserialize<Address>(msg);
                    OnAddress(address);
                    break;
            }

            return null;
        }        
    }
}
