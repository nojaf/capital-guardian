import React from 'react';
import {ElmishCapture, useBalance} from "./bin/Main"
import {Container, Jumbotron, Button } from "reactstrap"

const Loading = <p>loading</p>

function Meh() {
    const balance = useBalance()
    console.log(`value`, balance);
    return <p>Current balance: {balance}</p>
}

function App() {
    return (
        <ElmishCapture loading={Loading}>
            <Jumbotron>
                <Container>
                    <h1 className="display-3">Create react application with Fable</h1>
                    <p className="lead">This sample application illustrates how you can combine create-react-application &amp; Fable</p>
                    <Button color={"primary"} href={'https://github.com/nojaf/capital-guardian'}>Fork on GitHub</Button>
                </Container>
            </Jumbotron>
            <Container>
                <Meh/>
            </Container>
        </ElmishCapture>
    );
}

export default App;

// <div className="App">
//   <header className="App-header">
//     <img src={logo} className="App-logo" alt="logo" />
//     <p>
//       Edit <code>src/App.js</code> and save to reload.
//     </p>
//     <a
//       className="App-link"
//       href="https://reactjs.org"
//       target="_blank"
//       rel="noopener noreferrer"
//     >
//       Learn React
//     </a>
//   </header>
// </div>