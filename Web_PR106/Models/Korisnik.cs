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

	public class Korisnik
	{
		public string Username;
		public string Password;
		public string Name;
		public string Surname;
		public string Email;
		public string Date;
		public Gender Gender;
		public UserType UserType;
		public List<Rezervacija> ReservationList;

		public Korisnik() { }
	}
}