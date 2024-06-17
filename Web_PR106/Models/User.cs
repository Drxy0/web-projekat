using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public enum Gender
	{
		MALE,
		FEMALE
	}
	public enum UserType
	{
		PUTNIK,
		ADMINISTRATOR
	}

	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string Date { get; set; }
		public Gender Gender { get; set; }
		public UserType UserType {  get; set; }
		public List<Reservation> ReservationList { get; set; }

		public User() { }
	}
}