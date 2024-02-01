using Application.Core;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Domain;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Application.Activities;
using System.Linq;
using System;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Activities
                   .OrderBy(d => d.Date)
                   .Where(d=>d.Attendees.Any(c=>c.AppUser.UserName == request.Username))
                   .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider,new { currentUsername = request.Username })
                   .AsQueryable();

                if (request.Predicate == "past" && request.Predicate != "hosting")
                    query = query.Where(d => d.Date < DateTime.UtcNow);
                else if (request.Predicate != "past" && request.Predicate == "hosting")
                    query = query.Where(d => d.HostUsername == request.Username);
                else
                    query = query.Where(d => d.Date > DateTime.UtcNow);

                return Result<List<UserActivityDto>>.Success(await query.ToListAsync());
            }
        }
    }
}
