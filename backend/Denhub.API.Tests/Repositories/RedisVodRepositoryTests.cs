using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;
using Denhub.API.Repositories;
using Denhub.API.Services;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Denhub.API.Tests.Repositories {
    public class RedisVodRepositoryTests {
        [Fact]
        public async Task GetOrFetchVodsAsync_InitialFetchVods_VodsList() {
            var twitchClientMock = new Mock<ITwitchClient>();
            var redisClientMock = new Mock<IConnectionMultiplexer>();
            twitchClientMock
                .Setup(m => m.GetVideosByUserIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<TwitchVideoItemType>())).ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com"
                        }
                    },
                    Pagination = new TwitchPagination()
                });
            redisClientMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(() => {
                var db = new Mock<IDatabase>();
                db.Setup(m => m.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
                db.Setup(m => m.ListRangeAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<CommandFlags>())).ReturnsAsync(new List<RedisValue> {
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    }))
                }.ToArray());
                return db.Object;
            });
            var repo = new RedisVodRepository(twitchClientMock.Object, redisClientMock.Object);

            var result = await repo.GetOrFetchVodsAsync(1);

            Assert.Equal(3, result.Count());
        }
        
        [Fact]
        public async Task GetOrFetchVodsAsync_InitialFetchVodsFilter_FilteredVodsList() {
            var twitchClientMock = new Mock<ITwitchClient>();
            var redisClientMock = new Mock<IConnectionMultiplexer>();
            twitchClientMock
                .Setup(m => m.GetVideosByUserIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<TwitchVideoItemType>())).ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com",
                            Title = "test"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com",
                            Title = "test"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = DateTime.Now,
                            ThumbnailUrl = "test.com",
                            Title = "yep"
                        }
                    },
                    Pagination = new TwitchPagination()
                });
            redisClientMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(() => {
                var db = new Mock<IDatabase>();
                db.Setup(m => m.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
                db.Setup(m => m.ListRangeAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<CommandFlags>())).ReturnsAsync(new List<RedisValue> {
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "yep",
                        ThumbnailUrl = "test.com"
                    }))
                }.ToArray());
                return db.Object;
            });
            var repo = new RedisVodRepository(twitchClientMock.Object, redisClientMock.Object);

            var result = await repo.GetOrFetchVodsAsync(1, 0, 100, "test");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrFetchVodsAsync_UpToDateCache_VodsList() {
            var twitchClientMock = new Mock<ITwitchClient>();
            var redisClientMock = new Mock<IConnectionMultiplexer>();
            var time = DateTime.Now;
            twitchClientMock
                .Setup(m => m.GetVideosByUserIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<TwitchVideoItemType>())).ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        }
                    },
                    Pagination = new TwitchPagination()
                });
            redisClientMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(() => {
                var db = new Mock<IDatabase>();
                db.Setup(m => m.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
                db.Setup(m => m.ListGetByIndexAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync(JsonSerializer.Serialize(new CommonVodModel {
                        Date = time
                    }));
                db.Setup(m => m.ListRangeAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<CommandFlags>())).ReturnsAsync(new List<RedisValue> {
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    }))
                }.ToArray());
                return db.Object;
            });
            var repo = new RedisVodRepository(twitchClientMock.Object, redisClientMock.Object);

            var result = await repo.GetOrFetchVodsAsync(1);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetOrFetchVodsAsync_OutOfDateCache_VodsList() {
            var twitchClientMock = new Mock<ITwitchClient>();
            var redisClientMock = new Mock<IConnectionMultiplexer>();
            var time = DateTime.Now;
            twitchClientMock.SetupSequence(m => m.GetVideosByUserIdAsync(It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<TwitchVideoItemType>()))
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        }
                    },
                    Pagination = new TwitchPagination()
                })
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        }
                    },
                    Pagination = new TwitchPagination {
                        Cursor = "test"
                    }
                })
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem> {
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        },
                        new() {
                            Id = "1",
                            PublishedAt = time,
                            ThumbnailUrl = "test.com"
                        }
                    },
                    Pagination = new TwitchPagination()
                });
            redisClientMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(() => {
                var db = new Mock<IDatabase>();
                db.Setup(m => m.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
                db.Setup(m => m.ListGetByIndexAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync(JsonSerializer.Serialize(new CommonVodModel {
                        Date = time.AddMinutes(15)
                    }));
                db.Setup(m => m.ListRangeAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<CommandFlags>())).ReturnsAsync(new List<RedisValue> {
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    })),
                    new(JsonSerializer.Serialize(new CommonVodModel {
                        Title = "test",
                        ThumbnailUrl = "test.com"
                    }))
                }.ToArray());
                return db.Object;
            });
            var repo = new RedisVodRepository(twitchClientMock.Object, redisClientMock.Object);

            var result = await repo.GetOrFetchVodsAsync(1);

            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetOrFetchVodsAsync_VodsDoNotExist_EmptyList() {
            var twitchClientMock = new Mock<ITwitchClient>();
            var redisClientMock = new Mock<IConnectionMultiplexer>();
            redisClientMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(() => {
                    var dbMock = new Mock<IDatabase>();
                    dbMock.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(RedisValue.Null);
                    return dbMock.Object;
                });
            twitchClientMock.Setup(m => m.GetVideosByUserIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<TwitchVideoItemType>()))
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchVideoItem>> {
                    Data = new List<TwitchVideoItem>(),
                    Pagination = new TwitchPagination()
                });
            var repo = new RedisVodRepository(twitchClientMock.Object, redisClientMock.Object);

            var results = await repo.GetOrFetchVodsAsync(111111);

            Assert.Empty(results);
        }
    }
}