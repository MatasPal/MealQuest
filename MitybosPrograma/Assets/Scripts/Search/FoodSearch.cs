using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using TMPro;
using System.Data;
using System.Collections.Generic;

public class ProductDetails
{
    public int ProductID;
    public string Name;
    public float Kcal;
    public float Protein;
    public float Carbs;
    public float Fat;
}

public class FoodSearch : MonoBehaviour
{
    public ProgressBar progressBar_instance;

    

	public TMP_InputField searchInputField;
    public ScrollRect scrollView;

    public GameObject resultPrefab;
    public GameObject productDetailPanel;
    public GameObject searchPanel;
    public GameObject mainPanel;

    //Displaying product info
    public TMP_Text productNameText; // Text component to display product name
    public TMP_Text kcalText; // Text component to display kcal
    public TMP_Text proteinText; // Text component to display protein
    public TMP_Text carbsText; // Text component to display carbs
    public TMP_Text fatText; // Text component to display fat

    //Displaying total amounts
    public TMP_Text Total_kcalText; // Text component to display kcal
    public TMP_Text Total_kcal_header; // Text component to display kcal in the header element
    public TMP_Text Total_proteinText; // Text component to display protein
    public TMP_Text Total_carbsText; // Text component to display carbs
    public TMP_Text Totoal_fatText; // Text component to display fat

    //Displaying water (temp)
    public TMP_Text Water_field_plan;
    public TMP_Text Water_field_header;

    //Input field
    public TMP_InputField amountInputField;

    private ProductDetails selectedProduct;
    private List<ProductDetails> eatenProducts = new List<ProductDetails>(); // List to store eaten products

    private double water; //Temp variable for storing water amount localy

	public void AddWater()
	{
		water += 100;

		//int userId = SessionManager.GetIdKey();

		//string query = @"SELECT COUNT(*) FROM consumed_user_water WHERE id_user = " + userId + @";
  //                  ";
		//DBManager.OpenConnection();
		//IDbCommand command = DBManager.connection.CreateCommand();
		//command.CommandText = query;
		//object result = command.ExecuteScalar();
		//int count = result != null ? Convert.ToInt32(result) : 0;

		//if (count > 0)
		//{
		//	query = @"UPDATE consumed_user_water SET water = water + 100 WHERE id_user = " + userId + ";";
		//}
		//else
		//{
		//	query = @"INSERT INTO consumed_user_water (id_user, water) VALUES (" + userId + ", 100);";
		//}

		//command.CommandText = query;
		//command.ExecuteNonQuery();
	}


	public void DisplayWater()
	{
		//int userId = SessionManager.GetIdKey();

		//string query = @"SELECT water FROM consumed_user_water WHERE id_user = " + userId + ";";

		//DBManager.OpenConnection();
		//IDbCommand command = DBManager.connection.CreateCommand();
		//command.CommandText = query;
		//object result = command.ExecuteScalar();

		//int totalWater = result != null ? Convert.ToInt32(result) : 0;

		//Water_field_plan.text = "Total water: " + totalWater.ToString() + " ml";
		//Water_field_header.text = totalWater.ToString() + " ml";

		Water_field_plan.text = "Total water: " + water.ToString() + " ml";
		Water_field_header.text = water.ToString() + " ml";
	}

	void Start()
    {
		//removes data from consumed meals table for debugin only
		//RemoveDbDataFoodSearch();

		DisplayEatenProducts();
        DisplayWater();

		progressBar_instance.UpdateCurr();

		productDetailPanel.SetActive(false);
        searchPanel.SetActive(false);
    }

    void RemoveDbDataFoodSearch()
    {
		string query = "DELETE FROM consumed_user_meals";

		DBManager.OpenConnection();
		IDbCommand command = DBManager.connection.CreateCommand();
		command.CommandText = query;
		command.ExecuteNonQuery();
	}

	public void Search()
	{
		string input = searchInputField.text;

		string query = "SELECT * FROM product WHERE product_name LIKE '%" + input + "%';";

		try
		{
			DBManager.OpenConnection();
			IDbCommand command = DBManager.connection.CreateCommand();
			command.CommandText = query;
			IDataReader reader = command.ExecuteReader();

			ClearResultPanel();

			bool hasResult = false; // Flag to check if there's at least one result

			while (reader.Read())
			{
				hasResult = true;

				int product_id = int.Parse(reader[reader.GetOrdinal("id_product")].ToString());
				string name = reader[reader.GetOrdinal("product_name")].ToString();
				float kcal = float.Parse(reader[reader.GetOrdinal("kcal")].ToString());
				float protein = float.Parse(reader[reader.GetOrdinal("protein")].ToString());
				float carbs = float.Parse(reader[reader.GetOrdinal("carbohydrates")].ToString());
				float fat = float.Parse(reader[reader.GetOrdinal("fat")].ToString());

				GameObject resultObj = Instantiate(resultPrefab, scrollView.content);
				resultObj.GetComponentInChildren<TMP_Text>().text = name;

				// Add listener to the button
				resultObj.GetComponent<Button>().onClick.AddListener(() => OnSearchResultClicked(name, kcal, protein, carbs, fat, product_id));
			}

			if (!hasResult)
			{
				Debug.Log("No such food :((");
				GameObject resultObj = Instantiate(resultPrefab, scrollView.content);
				resultObj.GetComponentInChildren<TMP_Text>().text = "No such food :((";
			}

			reader.Close();
		}
		catch (Exception e)
		{
			Debug.LogError("Error searching for food products: " + e.Message);
		}
		finally
		{
			DBManager.connection.Close();
		}
	}


	void OnSearchResultClicked(string productName, float kcal, float protein, float carbs, float fat, int product_id)
    {
        // Populate selected product details
        selectedProduct = new ProductDetails
        {
            ProductID = product_id,
			Name = productName,
            Kcal = kcal,
            Protein = protein,
            Carbs = carbs,
            Fat = fat
        };


        searchPanel.SetActive(false);


        productNameText.text = productName;
        kcalText.text = "Kcal: " + kcal.ToString();
        proteinText.text = "Protein: " + protein.ToString() + "g";
        carbsText.text = "Carbs: " + carbs.ToString() + "g";
        fatText.text = "Fat: " + fat.ToString() + "g";


        productDetailPanel.SetActive(true);
    }

    public void CloseProductDetail()
    {
        productDetailPanel.SetActive(false);
    }

    void ClearResultPanel()
    {
        foreach (Transform child in scrollView.content)
        {
            Destroy(child.gameObject);
        }
    }

    public void GoToMain()
    {
        mainPanel.SetActive(true);
        productDetailPanel.SetActive(false);
        searchPanel.SetActive(false);

		searchInputField.text = "";
		foreach (Transform child in scrollView.content.transform)
		{
			Destroy(child.gameObject);
		}

		DisplayEatenProducts();
    }

    public void GoToSearch()
    {
        mainPanel.SetActive(false);
        productDetailPanel.SetActive(false);
        searchPanel.SetActive(true);
    }
    public void GoToDetailed()
    {
        mainPanel.SetActive(false);
        productDetailPanel.SetActive(true);
        searchPanel.SetActive(false);
    }

	public float allCalories;
    public void OnAddToEatenList()
    {
        int userId = SessionManager.GetIdKey();
        if (selectedProduct != null)
        {
            float amount = float.Parse(amountInputField.text);
            Debug.Log(amount);

            selectedProduct.Kcal = selectedProduct.Kcal * amount / 100;
            selectedProduct.Protein = selectedProduct.Protein * amount / 100;
            selectedProduct.Carbs = selectedProduct.Carbs * amount / 100;
            selectedProduct.Fat = selectedProduct.Fat * amount / 100;

			string query = "INSERT INTO consumed_user_meals (id_user, id_meal, kcal, protein,  fat, carbohydrates) " +
						   $"VALUES ({userId}, {selectedProduct.ProductID}, {selectedProduct.Kcal}, {selectedProduct.Protein},  {selectedProduct.Fat}, {selectedProduct.Carbs})";


			DBManager.OpenConnection();
			IDbCommand command = DBManager.connection.CreateCommand();
			command.CommandText = query;
			command.ExecuteNonQuery();


			eatenProducts.Add(selectedProduct);


			DisplayEatenProducts();
			//allCalories += selectedProduct.Kcal;
			//Debug.Log("All calories: " + allCalories);

			ReturnTotalKcal();
		}
        else
        {
            Debug.LogWarning("No product selected to add to eaten products list.");
        }
    }

    void DisplayEatenProducts()
    {
        float totalKcal = 0;
        float totalProtein = 0;
        float totalCarbs = 0;
        float totalFat = 0;
		int userId = SessionManager.GetIdKey();

		string query = "SELECT kcal, protein, carbohydrates, fat FROM consumed_user_meals WHERE id_user = " + userId;

		DBManager.OpenConnection();
		IDbCommand command = DBManager.connection.CreateCommand();
		command.CommandText = query;
		IDataReader reader = command.ExecuteReader();

		while (reader.Read())
		{
			totalKcal += Convert.ToSingle(reader["kcal"]);
			totalProtein += Convert.ToSingle(reader["protein"]);
			totalCarbs += Convert.ToSingle(reader["carbohydrates"]);
			totalFat += Convert.ToSingle(reader["fat"]);
		}

		Total_kcal_header.text = totalKcal.ToString() + " kcal";

        Total_kcalText.text = "Total Kcal: " + totalKcal.ToString();
        Total_proteinText.text = "Total Protein: " + totalProtein.ToString() + "g";
        Total_carbsText.text = "Total Carbs: " + totalCarbs.ToString() + "g";
        Totoal_fatText.text = "Total Fat: " + totalFat.ToString() + "g";
        allCalories = totalKcal;

        //PROGRES BAR STUFF DONT DELEETE
		progressBar_instance.curr = totalKcal;
		progressBar_instance.UpdateCurr();
	}

    public float ReturnTotalKcal()
    {
        return allCalories;
    }
}



