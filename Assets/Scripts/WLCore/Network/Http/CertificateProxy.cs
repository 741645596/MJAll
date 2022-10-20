// @Author: tanjinhua
// @Date: 2021/11/24  17:24


using System;
using UnityEngine.Networking;

namespace Unity.Network
{
    public class CertificateProxy : CertificateHandler
    {

        private Func<byte[], bool> _handler;

        public CertificateProxy(Func<byte[], bool> handler)
        {
            _handler = handler;
        }


        protected override bool ValidateCertificate(byte[] certificateData) => _handler(certificateData);
    }
}
