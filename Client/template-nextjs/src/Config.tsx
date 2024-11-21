import { useAppSelector } from "./hooks"
import Box from '@mui/material/Box';
import Slider from '@mui/material/Slider';
import React from "react";
import { useDispatch } from "react-redux";

type Props = {
    sendConfig: Function,
    store : any
}

interface Config {
    monsterAmount : number,
    monsterSpeed: number
    bombDelay: number
}
interface Admin {
    isAdmin : boolean
}
function Config(props: Props) {

    const dispatch = useDispatch();
    const [val, setVal] = React.useState<number>();
    const [isAdmin, setIsAdmin] = React.useState<boolean>()
    const cfg = useAppSelector((state) => state.config)
    const Admin: Admin = useAppSelector((state) => state.admin)

    React.useEffect(() => {
        setIsAdmin(Admin.isAdmin);
    }, [Admin]);
    
    const sendMonsterSpeed = (monsterSpeed: any) => 
    {
        const speed = monsterSpeed/10;
        props.sendConfig({monsterAmount: cfg.monsterAmount, monsterSpeed: speed, bombDelay: cfg.bombDelay});
    }
    return (
        <>
            {console.log(isAdmin)}
            {isAdmin && <h1>Admin</h1>}
            {!isAdmin && <h1>!Admin</h1>}
        {isAdmin && (
        <div>
            <Box sx={{ width: 300 }}>
                {cfg.bombDelay}
                <Slider onChangeCommitted={(e,v) => props.sendConfig({ monsterAmount: cfg.monsterAmount, monsterSpeed: cfg.monsterSpeed, bombDelay: v })} defaultValue={cfg.bombDelay} max={5000} aria-label="Default" valueLabelDisplay="auto" />
            </Box>
            <Box sx={{ width: 300 }}>
                {cfg.monsterAmount}
                <Slider onChange = {(e,v) => setVal(v as number) } value = {val} onChangeCommitted={(e,v) => props.sendConfig({ monsterAmount: v, monsterSpeed: cfg.monsterSpeed, bombDelay: cfg.bombDelay })}  defaultValue={cfg.monsterAmount} max={50} aria-label="Default" valueLabelDisplay="auto" />
            </Box>
            <Box sx={{ width: 300 }}>
                {cfg.monsterSpeed}
                <Slider onChangeCommitted={(e,v) => sendMonsterSpeed(v)} defaultValue={cfg.monsterSpeed*10} max={100.0} aria-label="Default" valueLabelDisplay="auto" />
            </Box>
        </div>)}
            {!isAdmin && (
                <div>
                    <Box sx={{ width: 300 }}>
                        {cfg.bombDelay}
                        <Slider value = {cfg.bombDelay} defaultValue={cfg.bombDelay} max={5000} aria-label="Default" valueLabelDisplay="auto" />
                    </Box>
                    <Box sx={{ width: 300 }}>
                        {cfg.monsterAmount}
                        <Slider value = {cfg.monsterAmount} defaultValue={cfg.monsterAmount} max={50} aria-label="Default" valueLabelDisplay="auto" />
                    </Box>
                    <Box sx={{ width: 300 }}>
                        {cfg.monsterSpeed}
                        <Slider value = {cfg.monsterSpeed} defaultValue={cfg.monsterSpeed*10} max={100.0} aria-label="Default" valueLabelDisplay="auto" />
                    </Box>
                </div>)}
        </>
    )
}

export default Config