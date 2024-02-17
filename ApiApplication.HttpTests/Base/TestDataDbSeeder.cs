using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ApiApplication.Core.Models;
using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.HttpTests.Base.FixtureExtensions;
using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ApiApplication.HttpTests.Base
{
    public class TestDataDbSeeder
    {
        private readonly IMapper _mapper;
        private readonly CinemaContext _dbContext;
        protected Fixture Fixture { get; }

        public TestDataDbSeeder(IMapper mapper, CinemaContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            Fixture = new Fixture();
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

        public int AddNewShowtimeToDatabase(int showtimeId = 1)
        {

            var auditoriumEntity = _dbContext.Auditoriums.Add(new AuditoriumEntity
            {
                Id = 1,
                Showtimes = new List<ShowtimeEntity> 
                { 
                    new ShowtimeEntity
                    {
                        Id = showtimeId,
                        SessionDate = new DateTime(2023, 1, 1),
                        Movie = new MovieEntity
                        {
                            Id = 1,
                            Title = "Inception",
                            ImdbId = "tt1375666",
                            ReleaseDate = new DateTime(2010, 01, 14),
                            Stars = "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page, Ken Watanabe"                            
                        },
                        AuditoriumId = 1,
                    } 
                },
                Seats = GenerateSeats(1, 28, 22)
            });
            
            
            // _dbContext.Showtimes.Add(showtimeEntity);
            _dbContext.SaveChanges();

            return showtimeId;
        }

        public void GenerateShowtimesToDatabase(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount can not be negative and 0");
            }

            var showtimeList = Fixture.GenerateListOf<Showtime>(amount);
            var showtimeEntityList = _mapper.Map<List<ShowtimeEntity>>(showtimeList);
            _dbContext.Showtimes.AddRange(showtimeEntityList);
            _dbContext.SaveChanges();
        }
        
        private static List<SeatEntity> GenerateSeats(int auditoriumId, short rows, short seatsPerRow)
        {
            var seats = new List<SeatEntity>();
            for (short r = 1; r <= rows; r++)
            for (short s = 1; s <= seatsPerRow; s++)
                seats.Add(new SeatEntity { AuditoriumId = auditoriumId, Row = r, SeatNumber = s });

            return seats;
        }

        public void AddNewMovieToDataBase(Movie movie)
        {
            if (movie is null)
            {
                throw new ArgumentException("Movie can not be null.");
            }

            var entity = _mapper.Map<MovieEntity>(movie);
            _dbContext.Movies.Add(entity);
            _dbContext.SaveChanges();
        }

        public bool CheckingExistingMovieWithImdbId(string id)
        {
            return _dbContext.Movies.FirstOrDefault(x => x.Id == 1) != null;
        }

        public void Clear()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}