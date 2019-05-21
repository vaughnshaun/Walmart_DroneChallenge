Assumptions

	- Based on the departure times in the sample, it appears that return time
		is being factored in. The return time seems to be about half the speed
		of the delivery speed. The assumption also is that there is no speed
		of loading a delivery on a drone.
	- There is only one warehouse (multiple in the future) that is located at center point (0, 0)
	- A drone could be represented in code, but to make things easy the warehouse
		handles speed estimates and shipping.
	- Order destinations should be translated to points. N and E are positive
		while S and W are negative.
	- Orders can come from any source, not just a text file. The program
		is structured in a way that will mimic a stream of orders.
	- The code should reflect design not read of a file.
	- The input should be correct or else the file will not generate.
		There isn't much input error checking. Time was spent mostly on designing.
		The assumption is that the input will match the sample exactly.
	- Storing the rating isn't needed only the algorithm for determining response type.
	- The program will stop if an order cannot be delivered within the end time.
		This assumption is that the program only has one days worth of data.

	- The solution to the problem requires a priority queue.
		1. Always deliver to the order that is created first.
		2. Unless, the order is so far away that other orders will
			possible miss out on the promoter time frame.
		3. The sort key is a combination of created and distance.
			The time to deliver an order is added to its created date.
			This will effectively push the order further down the list.
			Notice that an early date helps an order stay at the top while
			a short distance prevents an order from moving down the list.

App Design

	The goal of this design is to enable easy modification and single
	responsibility. Below is a description of each class

	DeliveredOrder - A display model for showing the final status
	of a delivered order.

	DeliveryService - This class is intended to be long running.
	Its main responsibility is to call methods on the OrderDeliverer
	class on an interval.

	IOrderStreamer - An interface for a stream of new orders. The
	implmentations can be file based, message based, and etc.

	Order - Represents an order that needs to be delivered.

	OrderDeliverer - This is the main class. This class handles
	most of the logic for sending an order out for delivery.

	OrderHelper - A utility class that contains methods for parsing order data and
	obtaining a delivered orders rating.

	OrderMockedTimeStreamer - An implementation of the IOrderStreamer. This
	particular implementation mimics the passing of time. It will return
	orders based on the current time being greater or equal to orders' created date.

	Point - A simple class that contains an X and Y.

	Program - The main entry point. This is mainly used for configuration
	and injection.

	Warehouse - Can queue an order for delivery. It also contains business
	logic for determining an order's distance, cost, and other delivery restrictions.
	
Test Design

	Test are using the Should_ExpectedBehavior_When_Condition naming convention.
	Based on the amount of time left to complete this project, only the most important
	classes have been tested. The Program class isn't a test priority since it is just
	configuration and no real logic is there. The assumption is that only some test are needed
	based on the time limit.