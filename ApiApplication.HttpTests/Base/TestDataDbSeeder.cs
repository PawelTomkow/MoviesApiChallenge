﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ApiApplication.Core.Models;
using ApiApplication.Database;
using ApiApplication.Database.Entities;
using AutoMapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.HttpTests.Base
{
    public class TestDataDbSeeder
    {
        private readonly IMapper _mapper;
        private readonly CinemaContext _dbContext;

        public TestDataDbSeeder(IMapper mapper, CinemaContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public int AddNewAuditoriumToDatabase(Auditorium auditorium)
        {
            if (auditorium is null)
            {
                throw new ArgumentException("Parameter 'auditorium' can not be null.");
            }
            
            var auditoriumEntity = _mapper.Map<AuditoriumEntity>(auditorium);
            
            _dbContext.Auditoriums.Add(auditoriumEntity);
            _dbContext.SaveChanges();

            return auditoriumEntity!.Id;
        }
        
        public void AddNewAuditoriumsToDatabase(List<Auditorium> auditoriums)
        {
            if (auditoriums is null || !auditoriums.Any())
            {
                throw new ArgumentException("Parameter 'auditorium' can not be null.");
            }
            
            var auditoriumEntity = _mapper.Map<List<AuditoriumEntity>>(auditoriums);
            
            _dbContext.Auditoriums.AddRange(auditoriumEntity);
            _dbContext.SaveChanges();
        }

        public int AddNewShowtimeToDatabase(Showtime showtime)
        {
            if (showtime is null)
            {
                throw new ArgumentException("Parameter 'showtime' can not be null.");
            }

            var showtimeEntity = _mapper.Map<ShowtimeEntity>(showtime);

            _dbContext.Showtimes.Add(showtimeEntity);
            _dbContext.SaveChanges();

            return showtimeEntity.Id;
        }
    }
}