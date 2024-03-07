﻿using System;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using static UnityEngine.UI.GridLayoutGroup;
using Mysqlx.Expr;
using Mysqlx.Crud;
using UnityEngine.Analytics;
using Unity.VisualScripting;
public class DatabaseMethods
{
  
	public int Login(string username, string password, out int id, string constring)
	{
        
        MySqlCommand command_login = new MySqlCommand();
        MySqlConnection ConnectionObject = new MySqlConnection();
        ConnectionObject.ConnectionString = constring;
        id = -1;
        try
        {
            command_login.CommandText = "SELECT * FROM food_db.user WHERE " +
                                    "username = @expr1 AND password = @expr2";
            command_login.Parameters.Add("@expr1", MySqlDbType.VarChar, 100).Value = username;
            command_login.Parameters.Add("@expr2", MySqlDbType.VarChar, 255).Value = password;
            command_login.Connection = ConnectionObject;
            ConnectionObject.Open();
            using (MySqlDataReader reader = command_login.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    reader.Read();
                    id = (int)reader.GetValue(0);
                    id = (int)reader.GetValue(0);
                    return id;
                }
                else
                    id = -1;
                return id;
            }

        }
        catch (MySqlException e)
        {
            System.Console.WriteLine(e.Message);
            return id;
        }
        catch (FormatException e)
        {
            System.Console.WriteLine("Bad format \n" + e.Message);
            return id;
        }
        finally
        {
            ConnectionObject.Close();
        }
    }

    public bool Register(string email, string username, string password, string constring)
    {
        MySqlCommand command_register = new MySqlCommand();
        MySqlCommand command_select = new MySqlCommand();
        MySqlConnection ConnectionObject = new MySqlConnection();
        ConnectionObject.ConnectionString = constring;
        int id = -1;
        try
        {
            command_register.CommandText = "INSERT INTO `food_db`.`user` " +
                                "(`email`,`username`,`password`) " +
                                "VALUES (@expr1, @expr2, @expr3)";
            command_register.Parameters.Add("@expr1", MySqlDbType.VarChar, 100).Value = email;
            command_register.Parameters.Add("@expr2", MySqlDbType.VarChar, 100).Value = username;
            command_register.Parameters.Add("@expr3", MySqlDbType.VarChar, 255).Value = password;

            command_select.CommandText = "SELECT `user`.`id_user` FROM `food_db`.`user`" +
                                        " WHERE username = @expr1 AND password = @expr2";
            command_select.Parameters.Add("@expr1", MySqlDbType.VarChar, 100).Value = username;
            command_select.Parameters.Add("@expr2", MySqlDbType.VarChar, 255).Value = password;

            

            command_register.Connection = ConnectionObject;
            command_select.Connection = ConnectionObject; 
            ConnectionObject.Open();
            
            
            int rowcount = command_register.ExecuteNonQuery();
            using (MySqlDataReader reader = command_select.ExecuteReader())
            {


                if (reader.HasRows)
                {
                    reader.Read();
                    id = (int)reader.GetValue(0);
                    
                    id = (int)reader.GetValue(0);
                    
                }
                else
                    id = -1;
            }





            if (rowcount > 0)
            {
                InsertRegisterPlaceholder(id, constring);
                return true;
            }
            else
                return false;
        }
        catch (MySqlException e)
        {
            System.Console.WriteLine(e.Message); return false;
        }
        finally { ConnectionObject.Close(); }
    }

    public void InsertRegisterPlaceholder(int id, string constring)
    {
        MySqlCommand command_insert = new MySqlCommand();
        MySqlConnection ConnectionObject = new MySqlConnection();
        ConnectionObject.ConnectionString = constring;
        try
        {
            command_insert.CommandText = "INSERT INTO `food_db`.`user_data` " +
                                        "(`id_user`,`height`,`weight`,`gender`,`goal`,`physical_activity`,`date_of_birth`)" +
                                        " VALUES(@expr1, 0.0, 0.0, 'Male', 'Lose weight', 1, CURRENT_DATE())";
            command_insert.Parameters.Add("@expr1", MySqlDbType.Int64, 6).Value = id;
            command_insert.Connection = ConnectionObject;
            ConnectionObject.Open();    
            command_insert.ExecuteNonQuery();
        }
        catch (MySqlException e)
        {
            System.Console.WriteLine(e.Message);
        }
        finally {ConnectionObject.Close(); }
    }

    public void UpdateProfile(int id, string gender, double height, double weight, string goal,int dateOfBirth, int activity, string constring)
    {
        MySqlCommand command_update = new MySqlCommand();
        MySqlConnection ConnectionObject = new MySqlConnection();
        ConnectionObject.ConnectionString = constring;
        //pakeisti kai prideti menesio ir dienos pasirinkimai.(metodas sukurti YYYY-mm-dd string);
        string year = dateOfBirth.ToString();
        try
        {
            command_update.CommandText = "UPDATE `food_db`.`user_data`" +
                                        " SET `height` = @expr2, `weight` = @expr3," +
                                        " `gender` = @expr4,  `goal` = @expr5, " +
                                        "`physical_activity` = @expr6, `date_of_birth` = @expr7 " +
                                        "WHERE `id_user` = @expr1";
            command_update.Parameters.Add("@expr1", MySqlDbType.Int64, 6).Value = id;
            command_update.Parameters.Add("@expr2", MySqlDbType.Decimal, 5).Value = height;
            command_update.Parameters.Add("@expr3", MySqlDbType.Decimal, 5).Value = weight;
            command_update.Parameters.Add("@expr4", MySqlDbType.Enum, 6).Value = gender;
            command_update.Parameters.Add("@expr5", MySqlDbType.Enum, 6).Value = goal;
            command_update.Parameters.Add("@expr6", MySqlDbType.Int64, 1).Value = activity;
            command_update.Parameters.Add("@expr7", MySqlDbType.Date, 10).Value = dateOfBirth + "-01-01";
            Debug.Log(dateOfBirth + "-01-01");


            command_update.Connection = ConnectionObject;
            ConnectionObject.Open();
            command_update.ExecuteNonQuery();
        }
        catch (MySqlException e) { System.Console.WriteLine(e.Message);}
        finally { ConnectionObject.Close(); }
    }
}
