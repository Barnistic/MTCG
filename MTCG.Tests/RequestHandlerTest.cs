using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MTCG.Models;
using MTCG.Repositories.Interfaces;
using MTCG.Services.Interfaces;
using MTCG.Infrastructure;
using NUnit.Framework;
using NSubstitute;

namespace MTCG.Tests.InfrastructureTests
{
    [TestFixture]
    public class RequestHandlerTest
    {
        private RequestHandler _requestHandler;
        private Dictionary<string, User> _users;
        private IUserRepository _userRepository;
        private ICardRepository _cardRepository;
        private IGameRepository _gameRepository;
        private ITradingRepository _tradingRepository;
        private IBattleService _battleService;
        private TcpListener _tcpListener;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        [SetUp]
        public void Setup()
        {
            _users = new Dictionary<string, User>();
            _userRepository = Substitute.For<IUserRepository>();
            _cardRepository = Substitute.For<ICardRepository>();
            _gameRepository = Substitute.For<IGameRepository>();
            _tradingRepository = Substitute.For<ITradingRepository>();
            _battleService = Substitute.For<IBattleService>();

            _tcpListener = new TcpListener(IPAddress.Loopback, 0);
            _tcpListener.Start();
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_tcpListener.LocalEndpoint as IPEndPoint);
            _networkStream = _tcpClient.GetStream();

            _requestHandler = Substitute.ForPartsOf<RequestHandler>(_users, _userRepository, _cardRepository, _gameRepository, _tradingRepository);
        }

        [TearDown]
        public void TearDown()
        {
            _networkStream.Close();
            _tcpClient.Close();
            _tcpListener.Stop();
        }

        [Test]
        public void HandleRequest_ShouldCallHandleSessionPost_ForSessionsPostRequest()
        {
            // Arrange
            string[] requestLines = new[]
            {
                "POST /sessions HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                "",
                "{\"Username\":\"test\",\"Password\":\"test\"}"
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleSessionPost(Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleUserPost_ForUsersPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /users HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                "",
                "{\"Username\":\"test\",\"Password\":\"test\"}"
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleUserPost(Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleUserGet_ForUsersGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /users/test HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleUserGet(Arg.Any<string>(), Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandlePackagePost_ForPackagesPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /packages HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                "",
                "[{\"Id\":\"1\",\"Name\":\"Card1\",\"Damage\":50,\"Type\":\"Fire\"}]"
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandlePackagePost(Arg.Any<User>(), Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleTransactionPost_ForTransactionsPackagesPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /transactions/packages HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleTransactionPost(Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleCardsGet_ForCardsGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /cards HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleCardsGet(Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleDeckGet_ForDeckGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /deck HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleDeckGet(Arg.Any<User>(), Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleStatsGet_ForStatsGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /stats HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleStatsGet(Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleScoreboardGet_ForScoreboardGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /scoreboard HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleScoreboardGet(Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleBattlePost_ForBattlesPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /battles HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleBattlePost(Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleTradingsGet_ForTradingsGetRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "GET /tradings HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleTradingsGet(Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleTradingPost_ForTradingsPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /tradings HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                "",
                "{\"Id\":\"1\",\"CardToTrade\":\"Card1\",\"Type\":\"Fire\",\"MinimumDamage\":50}"
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleTradingPost(Arg.Any<User>(), Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleTradingDelete_ForTradingsDeleteRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "DELETE /tradings/1 HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                ""
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleTradingDelete(Arg.Any<string>(), Arg.Any<User>(), Arg.Any<NetworkStream>());
        }

        [Test]
        public void HandleRequest_ShouldCallHandleTradingDealPost_ForTradingsDealPostRequest()
        {
            // Arrange
            var requestLines = new[]
            {
                "POST /tradings/1 HTTP/1.1",
                "Authorization: Bearer token",
                "Content-Type: application/json",
                "",
                "{\"CardToTrade\":\"Card1\"}"
            };

            // Act
            try
            {
                _requestHandler.HandleRequest(requestLines, _networkStream);
            }
            catch { }

            // Assert
            _requestHandler.Received().HandleTradingDealPost(Arg.Any<string>(), Arg.Any<User>(), Arg.Any<string[]>(), Arg.Any<NetworkStream>());
        }
    }
}





