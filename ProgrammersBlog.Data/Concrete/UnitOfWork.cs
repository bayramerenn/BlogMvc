﻿using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Data.Concrete.EntityFramework.Context;
using ProgrammersBlog.Data.Concrete.Repositories;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProgrammersBlogContext _context;
        private EfArticleRepository _articleRepository;
        private EfCategoryRepository _categoryRepository;
        private EfCommentRepository _commentRepository;

        public UnitOfWork(ProgrammersBlogContext context)
        {
            _context = context;
        }

        public IArticleRepository Articles => _articleRepository = _articleRepository ?? new EfArticleRepository(_context);
        public ICategoryRepository Categories =>  _categoryRepository ??= new EfCategoryRepository(_context);
        public ICommentRepository Comments => _commentRepository = _commentRepository ?? new EfCommentRepository(_context);




        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
