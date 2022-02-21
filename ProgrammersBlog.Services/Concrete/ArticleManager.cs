using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Entities.ComplexTypes;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Services.Utilities;
using ProgrammersBlog.Shared.Entities.Concrete;
using ProgrammersBlog.Shared.Utilities.Results;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Concrete
{
    public class ArticleManager : ManagerBase, IArticleService
    {
        private readonly UserManager<User> _userManager;

        public ArticleManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager) : base(unitOfWork, mapper)
        {
            _userManager = userManager;
        }

        public async Task<IResult> AddAsync(ArticleAddDto articleAddDto, string createdByName, int userId)
        {
            var article = Mapper.Map<Article>(articleAddDto);
            article.CreatedByName = createdByName;
            article.ModifiedByName = createdByName;
            article.UserId = userId;
            await UnitOfWork.Articles.AddAsync(article);
            await UnitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Article.Add(articleAddDto.Title));
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var articleCount = await UnitOfWork.Articles.CountAsync();
            if (articleCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, articleCount);
            }
            return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı", -1);
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var articleCount = await UnitOfWork.Articles.CountAsync(a => !a.IsDeleted);
            if (articleCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, articleCount);
            }
            return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı", -1);
        }

        public async Task<IResult> DeleteAsync(int articleId, string modifiedByName)
        {
            var result = await UnitOfWork.Articles.AnyAsync(a => a.ArticleId == articleId);

            if (result)
            {
                var article = await UnitOfWork.Articles.GetByIdAsync(articleId);
                article.IsDeleted = true;
                article.IsActive = false;
                article.ModifiedByName = modifiedByName;
                article.ModifiedDate = DateTime.Now;
                await UnitOfWork.Articles.UpdateAsync(article);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Article.Delete(article.Title));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IDataResult<ArticleDto>> GetAsync(int articleId)
        {
            var article = await UnitOfWork.Articles.GetAsync(a => a.ArticleId == articleId, a => a.Category, a => a.User);
            if (article != null)
            {
                article.Comments = await UnitOfWork.Comments.GetAllAsync(c => c.ArticleId == articleId && c.IsActive && !c.IsDeleted);
                return new DataResult<ArticleDto>(ResultStatus.Success, new ArticleDto
                {
                    ResultStatus = ResultStatus.Success,
                    Article = article
                });
            }
            return new DataResult<ArticleDto>(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IDataResult<ArticleListDto>> GetAllAsync(Expression<Func<Article, bool>> predicate, params Expression<Func<Article, object>>[] includeProperties)
        {
            var articles = await UnitOfWork.Articles.GetAllAsync(null, a => a.User, a => a.Category);

            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByCategoryAsync(int categoryId)
        {
            var result = await UnitOfWork.Articles.AnyAsync(a => a.CategoryId == categoryId);
            if (result)
            {
                var articles = await UnitOfWork.Articles.GetAllAsync(
                    a => a.CategoryId == categoryId && !a.IsDeleted && a.IsActive,
                    ar => ar.User,
                    ar => ar.Category
                    );
                if (articles.Count > -1)
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                    {
                        Articles = articles,
                        ResultStatus = ResultStatus.Success
                    });
                }
                return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true));
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNonDeletedAsync()
        {
            var articles = await UnitOfWork.Articles.GetAllAsync(a => !a.IsDeleted, ar => ar.User, ar => ar.Category);
            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var articles =
                await UnitOfWork.Articles.GetAllAsync(a => !a.IsDeleted && a.IsActive, ar => ar.User,
                    ar => ar.Category);
            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true), null);
        }

        public async Task<IResult> HardDeleteAsync(int articleId)
        {
            var result = await UnitOfWork.Articles.AnyAsync(a => a.ArticleId == articleId);
            if (result)
            {
                var article = await UnitOfWork.Articles.GetAsync(a => a.ArticleId == articleId);
                await UnitOfWork.Articles.DeleteAsync(article);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Article.HardDelete(article.Title));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IResult> UpdateAsync(ArticleUpdateDto articleUpdateDto, string modifiedByName)
        {
            var oldArticle = await UnitOfWork.Articles.GetByIdAsync(articleUpdateDto.ArticleId);
            var article = Mapper.Map<ArticleUpdateDto, Article>(articleUpdateDto, oldArticle);
            article.ModifiedByName = modifiedByName;
            await UnitOfWork.Articles.UpdateAsync(article);
            await UnitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Article.Update(article.Title));
        }

        public async Task<IDataResult<ArticleUpdateDto>> GetArticleUpdateDtoAsync(int articleId)
        {
            var result = await UnitOfWork.Articles.AnyAsync(a => a.ArticleId == articleId);
            if (result)
            {
                var article = await UnitOfWork.Articles.GetByIdAsync(articleId);
                var articleUpdateDto = Mapper.Map<ArticleUpdateDto>(article);
                return new DataResult<ArticleUpdateDto>(ResultStatus.Success, articleUpdateDto);
            }
            return new DataResult<ArticleUpdateDto>(ResultStatus.Error, Messages.Article.NotFound(false), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByDeletedAsync()
        {
            var articles = await UnitOfWork.Articles.GetAllAsync(a => a.IsDeleted, ar => ar.User, ar => ar.Category);
            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true), null);
        }

        public async Task<IResult> UndoDeleteAsync(int articleId, string modifiedByName)
        {
            var result = await UnitOfWork.Articles.AnyAsync(a => a.ArticleId == articleId);

            if (result)
            {
                var article = await UnitOfWork.Articles.GetByIdAsync(articleId);
                article.IsDeleted = false;
                article.IsActive = true;
                article.ModifiedByName = modifiedByName;
                article.ModifiedDate = DateTime.Now;
                await UnitOfWork.Articles.UpdateAsync(article);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Article.UndoDelete(article.Title));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByViewCountAsync(bool isAscending, int? takeSize)
        {
            var articles = await UnitOfWork.Articles.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.User);
            var sortedArticles = isAscending
                ? articles.OrderBy(a => a.ViewCount)
                : articles.OrderByDescending(a => a.ViewCount);
            return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
            {
                Articles = takeSize == null ? sortedArticles.ToList() : sortedArticles.Take(takeSize.Value).ToList(),
            });
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            pageSize = pageSize < 5 ? 5 : pageSize;
            var articles = categoryId == null
                ? await UnitOfWork.Articles.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User)
                : await UnitOfWork.Articles.GetAllAsync(a => a.CategoryId == categoryId.Value && a.IsActive && !a.IsDeleted, a => a.Category, a => a.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
            {
                Articles = sortedArticles,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = articles.Count,
                CategoryId = categoryId == null ? null : categoryId.Value,
                IsAscending = isAscending
            });
        }

        public async Task<IDataResult<ArticleListDto>> SearchAsync(string keyword, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            pageSize = pageSize < 5 ? 5 : pageSize;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var articles = await UnitOfWork.Articles.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User);

                var sortedArticles = isAscending
                    ? articles.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                    : articles.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = sortedArticles,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = articles.Count,
                    IsAscending = isAscending
                });
            }
            else
            {
                var searchedArticles = await UnitOfWork.Articles.SearchAsync(new List<Expression<Func<Article, bool>>>{
                    (a) => a.Title.Contains(keyword),
                    (a) => a.Category.Name.Contains(keyword),
                    (a) => a.SeoDescription.Contains(keyword),
                    (a) => a.SeoTags.Contains(keyword)
                }, a => a.User, a => a.Category);

                var searchedAndsortedArticles = isAscending
                   ? searchedArticles.OrderBy(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                   : searchedArticles.OrderByDescending(a => a.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
                {
                    Articles = searchedAndsortedArticles,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = searchedArticles.Count,
                    IsAscending = isAscending
                });
            }
        }

        public async Task<IResult> IncreaseViewCountAsync(int articleId)
        {
            var article = await UnitOfWork.Articles.GetByIdAsync(articleId);
            if (article != null)
            {
                article.ViewCount += 1;
                await UnitOfWork.Articles.UpdateAsync(article);
                await UnitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Article.IncreaseViewCount(article.Title));
            }
            return new Result(ResultStatus.Error, Messages.Article.NotFound(false));
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy, bool isAscending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount, int maxViewCount, int minCommentCount, int maxCommentCount)
        {
            var anyUser = await _userManager.Users.AnyAsync(u => u.Id == userId);
            if (!anyUser)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.User.NotFound(userId));
            }
            var userArticles = await UnitOfWork.Articles.GetAllAsync(a => a.UserId == userId && a.IsActive && !a.IsDeleted);

            List<Article> sortedArticles = new List<Article>();

            switch (filterBy)
            {
                case FilterBy.Category:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.CategoryId == categoryId).OrderBy(a => a.Date).Take(takeSize).ToList()
                                : userArticles.Where(a => a.CategoryId == categoryId).OrderByDescending(a => a.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.CategoryId == categoryId).OrderBy(a => a.ViewCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.CategoryId == categoryId).OrderByDescending(a => a.ViewCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.CategoryId == categoryId).OrderBy(a => a.CommentCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.CategoryId == categoryId).OrderByDescending(a => a.CommentCount).Take(takeSize).ToList();
                            break;
                    }
                    break;

                case FilterBy.Date:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderBy(a => a.Date).Take(takeSize).ToList()
                                : userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderByDescending(a => a.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderBy(a => a.ViewCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderByDescending(a => a.ViewCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderBy(a => a.CommentCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.Date >= startAt && a.Date <= endAt).OrderByDescending(a => a.CommentCount).Take(takeSize).ToList();
                            break;
                    }
                    break;

                case FilterBy.ViewCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderBy(a => a.Date).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderByDescending(a => a.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderBy(a => a.ViewCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderByDescending(a => a.ViewCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderBy(a => a.CommentCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minViewCount && a.ViewCount <= maxViewCount).OrderByDescending(a => a.CommentCount).Take(takeSize).ToList();
                            break;
                    }
                    break;

                case FilterBy.CommentCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderBy(a => a.Date).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderByDescending(a => a.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderBy(a => a.ViewCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderByDescending(a => a.ViewCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticles = isAscending
                                ? userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderBy(a => a.CommentCount).Take(takeSize).ToList()
                                : userArticles.Where(a => a.ViewCount >= minCommentCount && a.ViewCount <= maxCommentCount).OrderByDescending(a => a.CommentCount).Take(takeSize).ToList();
                            break;
                    }
                    break;
            }

            return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
            {
                Articles = sortedArticles
            });
        }

        public async Task<IDataResult<ArticleDto>> GetByIdAsync(int articleId, bool includeCategory, bool includeComment, bool includerUser)
        {
            List<Expression<Func<Article, bool>>> predicates = new List<Expression<Func<Article, bool>>>();
            List<Expression<Func<Article, object>>> includes = new List<Expression<Func<Article, object>>>();

            if (includeCategory) includes.Add(a => a.Category);
            if (includeComment) includes.Add(a => a.CommentCount);
            if (includerUser) includes.Add(a => a.User);
            predicates.Add(a => a.ArticleId == articleId);
            var article = await UnitOfWork.Articles.GetAsyncV2(predicates, includes);

            if (article == null)
            {
                return new DataResult<ArticleDto>(ResultStatus.Warning, Messages.General.ValidationError(), null, new List<ValidationError>
                {
                    new ValidationError
                    {
                        PropertyName = "articleId",
                        Message = Messages.Article.NotFoundById(articleId)
                    }
                });
            }
            return new DataResult<ArticleDto>(ResultStatus.Success, new ArticleDto
            {
                Article = article
            });
        }

        public async Task<IDataResult<ArticleListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted, int currentPage, int pageSize, OrderByGeneral orderByGeneral, bool isAscending, bool includeCategory, bool includeComment, bool includerUser)
        {
            List<Expression<Func<Article, bool>>> predicates = new List<Expression<Func<Article, bool>>>();
            List<Expression<Func<Article, object>>> includes = new List<Expression<Func<Article, object>>>();

            if (categoryId.HasValue)
            {
                if (await UnitOfWork.Categories.AnyAsync(c => c.CategoryId == categoryId.Value))
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Warning, Messages.General.ValidationError(), null, new List<ValidationError>
                    {
                        new ValidationError
                        {
                            PropertyName = "categoryId",
                            Message = Messages.Category.NotFoundById(categoryId.Value)
                        }
                    });
                }
                predicates.Add(a => a.CategoryId == categoryId);
            }
            if (userId.HasValue)
            {
                if (await _userManager.Users.AnyAsync(u => u.Id == userId.Value))
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Warning, Messages.General.ValidationError(), null, new List<ValidationError>
                    {
                        new ValidationError
                        {
                            PropertyName = "userId",
                            Message = Messages.Category.NotFoundById(userId.Value)
                        }
                    });
                }
                predicates.Add(a => a.UserId == userId);
            }
            if (isActive.HasValue) predicates.Add(a => a.IsActive == isActive.Value);
            if (isDeleted.HasValue) predicates.Add(a => a.IsDeleted == isDeleted.Value);

            if (includeCategory) includes.Add(a => a.Category);
            if (includeComment) includes.Add(a => a.CommentCount);
            if (includerUser) includes.Add(a => a.User);

            var articles = await UnitOfWork.Articles.GetAllAsyncV2(predicates, includes);

            IOrderedEnumerable<Article> sortedArticles = null;

            switch (orderByGeneral)
            {
                case OrderByGeneral.Id:
                    sortedArticles = isAscending ? articles.OrderBy(a => a.ArticleId) : articles.OrderByDescending(a => a.ArticleId);
                    break;
                case OrderByGeneral.Az:
                    sortedArticles = isAscending ? articles.OrderBy(a => a.Title) : articles.OrderByDescending(a => a.Title);
                    break;
                case OrderByGeneral.CreatedDate:
                    sortedArticles = isAscending ? articles.OrderBy(a => a.CreatedDate) : articles.OrderByDescending(a => a.CreatedDate);
                    break;

            }



            return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
            {
                Articles = sortedArticles.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(),
                CategoryId = categoryId.HasValue ? categoryId.Value : null,
                CurrentPage = currentPage,
                IsAscending = isAscending,
                PageSize = pageSize,
                ResultStatus = ResultStatus.Success,
                TotalCount = sortedArticles.Count()
            });
        }
    }
}