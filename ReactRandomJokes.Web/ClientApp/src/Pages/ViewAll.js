import React from 'react';
import axios from 'axios';

class ViewAll extends React.Component {
    state = {
        jokes: []
    }

    componentDidMount = async () => {
        const { data } = await axios.get('/api/jokes/viewall');
        this.setState({ jokes: data });
    }

    render() {
        return (
            <div className="row">
                <div className="col-md-6 col-md-offset-3">
                    {this.state.jokes.map(joke => {
                        return (
                            <div key={joke.id} className="well">
                                <h5>{joke.setup}</h5>
                                <h5>{joke.punchline}</h5>
                                <span>Likes: {joke.userJokeLikes.filter(j => j.like).length}</span>
                                <br />
                                <span>Dislikes:  {joke.userJokeLikes.filter(j => !j.like).length}</span>
                            </div>
                        )
                    })}
                </div>
            </div>
        )
    }
}

export default ViewAll;