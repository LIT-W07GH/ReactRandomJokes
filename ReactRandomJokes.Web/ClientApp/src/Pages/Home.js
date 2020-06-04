import React from 'react';
import { AuthContext } from '../AuthContext';
import { Link } from 'react-router-dom';
import axios from 'axios';
import produce from 'immer';

class Home extends React.Component {

    state = {
        joke: {
            id: '',
            setup: '',
            punchline: '',
            likesCount: '',
            dislikesCount: ''
        },
        userInteractionStatus: '',
        intervalObj: ''
    }

    componentDidMount = async () => {
        const { data } = await axios.get('/api/jokes/randomjoke');
        const { data: interactionStatus } = await axios.get(`/api/jokes/getinteractionstatus/${data.id}`);
        this.setState({ joke: data, userInteractionStatus: interactionStatus.status });
        const intervalObj = setInterval(() => {
            this.updateCounts();
        }, 500);
        this.setState({ intervalObj });
    }

    componentWillUnmount = () => {
        clearInterval(this.state.intervalObj);
    }

    updateCounts = async () => {
        const { id } = this.state.joke;
        if (!id) {
            return;
        }
        const { data } = await axios.get(`/api/jokes/getlikescount/${id}`);
        const nextState = produce(this.state, draft => {
            draft.joke.likesCount = data.likes;
            draft.joke.dislikesCount = data.dislikes;
        });
        this.setState(nextState);
    }

    interactWithJoke = async like => {
        const { id } = this.state.joke;
        await axios.post(`/api/jokes/interactwithjoke`, { jokeId: id, like });
        const { data: interactionStatus } = await axios.get(`/api/jokes/getinteractionstatus/${id}`);
        this.setState({ userInteractionStatus: interactionStatus.status });
    }

    render() {
        const { setup, punchline, likesCount, dislikesCount } = this.state.joke;
        const { userInteractionStatus } = this.state;
        const canLike = userInteractionStatus !== 'Liked' && userInteractionStatus !== 'CanNoLongerInteract';
        const canDislike = userInteractionStatus !== 'Disliked' && userInteractionStatus !== 'CanNoLongerInteract';
        return (
            <div className="row">
                <div className="col-md-6 col-md-offset-3 well">
                    {setup && <div>
                        <h4>{setup}</h4>
                        <h4>{punchline}</h4>
                        <div>
                            {userInteractionStatus !== 'Unauthenticated' && <div>
                                <button disabled={!canLike} onClick={() => this.interactWithJoke(true)} className="btn btn-primary">Like</button>
                                <button disabled={!canDislike} onClick={() => this.interactWithJoke(false)} className="btn btn-danger">Dislike</button>
                            </div>}
                            {userInteractionStatus === 'Unauthenticated' && <div>
                                <Link to='/login'>Login to your account to like/dislike this joke</Link>
                            </div>}
                            <br />
                            <h4>Likes: {likesCount}</h4>
                            <h4>Dislikes: {dislikesCount}</h4>
                            <h4>
                                <button className='btn btn-link' onClick={() => window.location.reload()}>Refresh</button>
                            </h4>
                        </div>
                    </div>}
                    {!setup && <h3>Loading...</h3>}
                </div>
            </div>

        )
    }
}

export default Home;