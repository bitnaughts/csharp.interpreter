public class ProcessorObject {

	public float tick_speed;

	public ProcessorObject () {
		tick_speed = .05f; //in seconds
	}
	public ProcessorObject (float tick_speed) {
		this.tick_speed = tick_speed;
	}
	public void setSpeed (float tick_speed) {
		this.tick_speed = tick_speed;
	}

}