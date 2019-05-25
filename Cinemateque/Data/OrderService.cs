using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models;
using Cinemateque.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemateque.Data
{
    public class OrderService : IOrderService
    {
        private readonly CinematequeContext _context;

        public CinematequeContext Context => _context;

        public OrderService(CinematequeContext cont)
        {
            _context = cont;
        }

        public async Task UpdateOrder(Order order)
        {
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }

        public Task<Order> GetOrder(int id)
        {
            return _context.Order.Include(or => or.Film).FirstOrDefaultAsync(prod => prod.OrderId == id);

        }

        public async Task<CartViewModel> GetCart(int userId)
        {
            var orders = _context.Order.Where(o => o.UserId == userId).Include(prod => prod.Film);
            var result = new CartViewModel
            {
                Orders = orders.ToArray(),
                TotalSum = orders.Sum(c => c.Film.Price - c.Film.Price * c.Film.Discount / 100) ?? 0,

            };
            return result;
        }

        public async Task<Order> AddOrder(Order order)
        {
            var result = await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task RemoveOrder(int id)
        {
            var ord = _context.Order.FirstOrDefault(pr => pr.OrderId == id);
            _context.Order.Remove(ord);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<Order> GetUserOrders(int userId)
        {
            var res = _context.Order.Where(or => or.UserId == userId).Include(or => or.Film).ToList();
            return res;
        }

    }

    public interface IOrderService
    {
        Task<Order> GetOrder(int id);
        Task<CartViewModel> GetCart(int userId);
        Task<Order> AddOrder(Order order);
        Task RemoveOrder(int id);
        IEnumerable<Order> GetUserOrders(int userId);
        Task UpdateOrder(Order order);
    }
}
