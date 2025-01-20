import imageStar from '../public/assets/star.png'
import { useAppSelector } from './hooks'
import Image from 'next/image'

interface PlayerStats {
    lives : number,
    bombs : number,
    range : number,
    speed : number
}

function Stats() {

    const Stats: PlayerStats = useAppSelector((state) => state.playerStats)
    
    return(
        <div>
            <div>
                <img src={window.location.origin + '/assets/star.png'} width={40} height={40} /> Speed: {Stats.speed}
            </div>
            <div>
                <img src={window.location.origin + '/assets/1up.png'} width={40} height={40} /> Lives: {Stats.lives}
            </div>
            <div>
                <img src={window.location.origin + '/assets/bomb.png'} width={40} height={40} /> Bombs: {Stats.bombs}
            </div>
            <div>
                <img src={window.location.origin + '/assets/fire.png'} width={40} height={40} /> Range: {Stats.range}
            </div>
        </div>
    )
}

export default Stats