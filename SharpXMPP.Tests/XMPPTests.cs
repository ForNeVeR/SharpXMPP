﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpXMPP.XMPP;
using SharpXMPP.XMPP.Stream;
using SharpXMPP.XMPP.Stream.Elements;

namespace SharpXMPP.Tests
{
    [TestClass]
    public class XMPPTests
    {
        static void TestJID(JID jid, string user, string domain, string resource)
        {
            Assert.AreEqual(user, jid.User);
            Assert.AreEqual(domain, jid.Domain);
            Assert.AreEqual(resource, jid.Resource);
        }
        [TestMethod]
        public void JIDTests()
        {
            TestJID(new JID("_vt@xmpp.ru/ololo"), "_vt", "xmpp.ru", "ololo");
            TestJID(new JID("icq.jabber.ru"), string.Empty, "icq.jabber.ru", null);
            TestJID(new JID("vasya@icq.org"), "vasya", "icq.org", null);
            Assert.AreEqual("vasya@icq.org", new JID("vasya@icq.org").ToString());
            Assert.AreEqual("icq.org", new JID("icq.org").ToString());
            Assert.AreEqual("icq.org/registered", new JID("icq.org/registered").ToString());
        }
        [TestMethod]
        public void DNSTests()
        {
            DNS.ResolveXMPPClient("gmail.com").ForEach(r => Trace.WriteLine(r.Host + ":" + r.Port));
        }

        [TestMethod]
        public void StanzaTests()
        {
            const string xmldata = "<stream:error xmlns:stream=\"http://etherx.jabber.org/streams\"><not-well-formed xmlns=\"urn:ietf:params:xml:ns:xmpp-streams\" /></stream:error>";
            var errorinput = XElement.Parse(xmldata);
            var payload  = Stanza.Clone<Error>(errorinput);
            Assert.AreEqual(StreamErrorType.NotWellFormed, payload.ErrorType);

            var error = new Error {ErrorType = StreamErrorType.NotWellFormed};
            // Remove all namespace attributes.
            error.DescendantsAndSelf().Attributes().Where(n => n.IsNamespaceDeclaration).Remove();

            // Specify that the namespace will be serialized with a namespace prefix of 'b'.
            error.Add(new XAttribute(XNamespace.Xmlns + "stream", Namespaces.Streams));
            Assert.AreEqual(payload.ToString(), Stanza.Clone<Error>(XElement.Parse(error.ToString())).ToString());
        }

    }
}
