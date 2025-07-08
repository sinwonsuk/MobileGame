using BackEnd;
public class FoodData
{
	public string tableName = "FOODS";
	public string foodName;
	public string foodImagePath;
	public int foodPrice;

	public FoodData() {}
	
	public FoodData(string foodName, string foodImagePath, int foodPrice)
	{
		this.foodName = foodName;
		this.foodImagePath = foodImagePath;
		this.foodPrice = foodPrice;
	}

	public Param ToParam()
	{
		Param param = new Param();
		param.Add("foodName", foodName);
		param.Add("foodImagePath", foodImagePath);
		param.Add("foodPrice", foodPrice);
		return param;
	}
}

