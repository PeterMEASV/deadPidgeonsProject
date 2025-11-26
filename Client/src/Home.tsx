import {useNavigate} from "react-router";

function Home() {
    const navigate = useNavigate();

    return (
        <>
            <h1>Home</h1>
            <button onClick={() => navigate("/template")}>Go to Template</button>
        </>
    )
}
export default Home