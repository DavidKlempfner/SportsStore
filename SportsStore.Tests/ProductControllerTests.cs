using Moq;
using SportsStore.Models;
using System;
using Xunit;
using System.Linq;
using SportsStore.Controllers;
using System.Collections;
using System.Collections.Generic;
using SportsStore.Models.ViewModels;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product{ ProductId = 1, Name = "P1"},
                new Product{ ProductId = 2, Name = "P2"},
                new Product{ ProductId = 3, Name = "P3"},
                new Product{ ProductId = 4, Name = "P4"},
                new Product{ ProductId = 5, Name = "P5"}
            }).AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductId = 1, Name = "P1"},
                new Product { ProductId = 2, Name = "P2"},
                new Product { ProductId = 3, Name = "P3"},
                new Product { ProductId = 4, Name = "P4"},
                new Product { ProductId = 5, Name = "P5"}
            }).AsQueryable<Product>());

            //Arrange
            ProductController controller = new ProductController(mock.Object)
            {
                PageSize = 3
            };

            //Act
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            //Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.Equal(2, pagingInfo.CurrentPage);
            Assert.Equal(3, pagingInfo.ItemsPerPage);
            Assert.Equal(5, pagingInfo.TotalItems);
            Assert.Equal(2, pagingInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Products()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductId = 1, Category = "Cat1", Name = "P1"},
                new Product { ProductId = 2, Category = "Cat2", Name = "P2"},
                new Product { ProductId = 3, Category = "Cat1", Name = "P3"},
                new Product { ProductId = 4, Category = "Cat2", Name = "P4"},
                new Product { ProductId = 5, Category = "Cat3", Name = "P5"}
            }).AsQueryable<Product>());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            Product[] result = (controller.List("Cat2", 1).ViewData.Model as ProductsListViewModel).Products.ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
    }
}