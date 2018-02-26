using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Commissionor.WebApi.Controllers;
using Commissionor.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace Commissionor.WebApi.Tests.Controllers
{
    public class ReadersControllerTests
    {
        [Fact]
        public async Task Create_creates_reader_in_DB()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb()) {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var reader = new Reader()
                {
                    Placement = "Placement",
                    Description = "Description"
                };

                // Act
                var readerExistedBefore = await dbContext.Readers.AnyAsync(r => r.Id == readerId);
                var result = await controller.Create(readerId, reader);
                var createdReader = await dbContext.Readers.SingleOrDefaultAsync(r => r.Id == readerId);

                // Assert
                Assert.False(readerExistedBefore);
                Assert.IsType<OkResult>(result);
                Assert.NotNull(createdReader);
                Assert.Equal(readerId, createdReader.Id);
                Assert.Equal(reader.Placement, createdReader.Placement);
                Assert.Equal(reader.Description, createdReader.Description);   
            }
        }

        [Fact]
        public async Task Create_returns_Conflict_if_readerId_exists_in_DB_already()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var reader = new Reader()
                {
                    Id = readerId,
                    Placement = "Placement",
                    Description = "Description"
                };
                await dbContext.Readers.AddAsync(reader);
                await dbContext.SaveChangesAsync();

                // Act
                var readerExistedBefore = await dbContext.Readers.AnyAsync(r => r.Id == readerId);
                var result = await controller.Create(readerId, reader);

                // Assert
                Assert.True(readerExistedBefore);
                Assert.IsType<StatusCodeResult>(result);
                Assert.Equal((int)HttpStatusCode.Conflict, ((StatusCodeResult)result).StatusCode);
            }
        }

        [Fact]
        public async Task Create_with_mismatched_readerIds_returns_BadRequest()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var reader = new Reader()
                {
                    Id = readerId + "2",
                    Placement = "Placement",
                    Description = "Description"
                };

                // Act
                var result = await controller.Create(readerId, reader);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        [Fact]
        public async Task AddLocation_adds_reader_location_to_DB()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var reader = new Reader()
                {
                    Id = readerId,
                    Placement = "Placement",
                    Description = "Description"
                };
                await dbContext.Readers.AddAsync(reader);
                await dbContext.SaveChangesAsync();

                var location = new Location()
                {
                    Site = "Site",
                    Room = "Room",
                    Door = "Door"
                };

                // Act
                var locationExistedBefore = await dbContext.Locations.AnyAsync();
                var result = await controller.AddLocation(readerId, location);
                var createdLocation = await dbContext.Locations.SingleOrDefaultAsync();

                // Assert
                Assert.False(locationExistedBefore);
                Assert.IsType<OkResult>(result);
                Assert.NotNull(createdLocation);
                Assert.Equal(readerId, createdLocation.ReaderId);
                Assert.Equal(location.Site, createdLocation.Site);
                Assert.Equal(location.Room, createdLocation.Room);
                Assert.Equal(location.Door, createdLocation.Door);
            }
        }

        [Fact]
        public async Task AddLocation_with_unknown_readerId_returns_NotFound()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var location = new Location()
                {
                    Site = "Site",
                    Room = "Room",
                    Door = "Door"
                };

                // Act
                var result = await controller.AddLocation(readerId, location);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task AddLocation_with_mismatched_readerIds_returns_BadRequest()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);
                const string readerId = "readerId";
                var reader = new Reader()
                {
                    Id = readerId,
                    Placement = "Placement",
                    Description = "Description"
                };
                await dbContext.Readers.AddAsync(reader);
                await dbContext.SaveChangesAsync();

                var location = new Location()
                {
                    ReaderId = readerId + "2",
                    Site = "Site",
                    Room = "Room",
                    Door = "Door"
                };

                // Act
                var result = await controller.AddLocation(readerId, location);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        [Fact]
        public async Task Get_returns_BadRequest_if_no_readerId_specified()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);

                // Act
                var result = await controller.Get(null);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }
        }

        [Fact]
        public async Task Get_returns_reader_data_including_locations()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var reader = new Reader()
                {
                    Id = "ReaderId",
                    Description = "Description",
                    Placement = "Placement",
                    Locations = new List<Location>() {
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site1",
                            Room = "Room1",
                            Door = "Door1"
                        },
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site2",
                            Room = "Room2",
                            Door = "Door2"
                        },
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site3",
                            Room = "Room3",
                            Door = "Door3"
                        }
                    }
                };
                dbContext.Readers.Add(reader);
                await dbContext.SaveChangesAsync();

                var controller = CreateController(dbContext);

                // Act
                var result = await controller.Get("ReaderId");

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var resultData = ((OkObjectResult)result).Value;
                resultData.Should().BeOfType<Reader>();
                var resultReader = (Reader)resultData;
                resultReader.Should().BeEquivalentTo(reader);
            }
        }

        [Fact]
        public async Task Get_returns_NotFound_if_readerId_does_not_exist_in_DB()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);

                // Act
                var result = await controller.Get("ReaderId");

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public async Task Delete_returns_BadRequest_if_no_readerId_specified()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);

                // Act
                var result = await controller.Delete(null);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }
        }

        [Fact]
        public async Task Delete_deletes_reader_including_locations()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var reader = new Reader()
                {
                    Id = "ReaderId",
                    Description = "Description",
                    Placement = "Placement",
                    Locations = new List<Location>() {
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site1",
                            Room = "Room1",
                            Door = "Door1"
                        },
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site2",
                            Room = "Room2",
                            Door = "Door2"
                        },
                        new Location() {
                            ReaderId = "ReaderId",
                            Site = "Site3",
                            Room = "Room3",
                            Door = "Door3"
                        }
                    }
                };
                dbContext.Readers.Add(reader);
                await dbContext.SaveChangesAsync();

                var controller = CreateController(dbContext);

                // Act
                await controller.Delete("ReaderId");

                // Assert
                var deletedReader = await dbContext.Readers.SingleOrDefaultAsync(r => r.Id == "ReaderId");
                deletedReader.Should().BeNull();
                var deletedLocations = await dbContext.Locations.Where(l => l.ReaderId == "ReaderId").ToListAsync();
                deletedLocations.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Delete_returns_NotFound_if_readerId_does_not_exist_in_DB()
        {
            // Arrange
            using (var dbContext = await TestUtils.CreateTestDb())
            {
                var controller = CreateController(dbContext);

                // Act
                var result = await controller.Delete("ReaderId");

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }

        static ReadersController CreateController(CommissionorDbContext dbContext)
        {
            //https://github.com/aspnet/Mvc/issues/6000#issuecomment-346756781
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));

            var controller = new ReadersController(dbContext);
            controller.ObjectValidator = objectValidator.Object;
            return controller;
        }
    }
}
