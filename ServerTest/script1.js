function clock_status()

         {

         window.setTimeout("clock_status()",100);

	 today=new Date();

	 self.status=today.toString();

	 }