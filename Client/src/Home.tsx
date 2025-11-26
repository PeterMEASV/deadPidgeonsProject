import {useNavigate} from "react-router";

function Home() {
    const navigate = useNavigate();

    return (
        <>
            <h1>Navigate to:</h1>
            <button onClick={() => navigate("/Admin")}>Admin Page</button>
            <button onClick={() => navigate("/Player")}>Player Page</button>
        </>
    )
}
export default Home