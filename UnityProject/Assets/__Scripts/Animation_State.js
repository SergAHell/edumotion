public var animState = 0;


function Update (){


switch (animState) {

	case 0:
	animation.CrossFade("idle");
	break;
	
	case 1:
	animation.CrossFade("punch");
	break;
	
	case 2:
	animation.CrossFade("armsUp");
	break;
	
	case 3:
	animation.CrossFade("sword");
	break;
	
	}
	
}
	



