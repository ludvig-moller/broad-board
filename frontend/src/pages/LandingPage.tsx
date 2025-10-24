import StartBoardButton from "../components/StartBoardButton";

export default function LandingPage() {
    return (
        <div className="landing-page-container">
            <StartBoardButton />
            <div className="landing-page-info">
                <p>Press Start a Board and start drawing!</p>
                <p>BroadBoard lets multiple users draw together in real time on shared boards.</p>
            </div>
        </div>
    );
}
